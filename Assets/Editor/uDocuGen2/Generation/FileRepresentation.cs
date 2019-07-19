using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using uDocumentGenerator.Helpers;
using UnityEngine;

namespace uDocumentGenerator.Generation
{
    /// <summary>
    /// This class represents a file, which is used to generate documentation. It includes the important features of a file.
    /// </summary>
    public class FileRepresentation
    {
        // The nameSpaces the class imports
        public List<string> imports = new List<string>();
        // Keys are access modifiers and values are nested lists of variable names
        public Dictionary<string, List<VariableRepresentation>> variables = new Dictionary<string, List<VariableRepresentation>>();
        // A list of objects the class inherits/implements
        public List<string> inheritance = new List<string>();
        // The access modifier of the class
        public string accessModifier;
        // The name of the class
        public string className;
        // The description of the class
        public string description;
        // The path of the file
        public string filePath;
        // The nameSpace of the class
        public string nameSpace = "";
        // How the class is declared
        public string declaration = "";
        /// <summary>
        /// Values are formatted:\n\n
        ///
        /// 1. ```Functions[key][0]``` is a list of strings containing param type and name as an element.\n
        /// 2. ```Functions[key][1]``` is the function name.\n
        /// 3. ```Functions[key][2]``` is a list of other modifiers.
        /// </summary>
        public Dictionary<string, List<FunctionRepresentation>> functions = new Dictionary<string, List<FunctionRepresentation>>();
        [JsonIgnore]
        // The ```FileReader``` that reads the file
        private readonly FileReader fileReader;
        // A list of C# access modifiers to reference
        [JsonIgnore]
        private readonly string[] accessMods = new string[] { "public", "protected", "private", "internal", "protected internal", "private protected" };
        // Is the file an interface? 
        [JsonIgnore]
        private bool isInterface = false;
        // Is the class abstract?
        [JsonIgnore]
        private bool isAbstract = false;
        public FileRepresentation(string fp)    
        {
            filePath = fp;
            fileReader = new FileReader(fp);
            ExtractImports();
            ExtractClassInformation();
            ExtractVariables();
            ExtractFunctions();
            fileReader.Reset();
        }
        /// <summary>
        /// Extracts functions from the file and turns them into ```FunctionRepresentation``` objects.
        /// </summary>
        private void ExtractFunctions()
        {

            Type classType = GetType(nameSpace + "." + className);
            var methodInfos = classType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var textArray = File.ReadAllLines(filePath).ToList();
            var commentTypes = new string[] { "///", "//", "/*", "*", "*/" };

            foreach (var method in methodInfos)
            {
                string methodAccessMod = "default";
                string methodName = method.Name;
                var methodModifiers = new List<string>();
                var dirty_parameter_list = new List<(string, string, object)>();
                var clean_parameter_list = new List<(string, object)>();
                if (method.IsPublic)
                    methodAccessMod = "public";
                else if (method.IsPrivate)
                    methodAccessMod = "private";
                else if (method.IsAssembly)
                    methodAccessMod = "internal";
                else if (method.IsFamily)
                    methodAccessMod = "protected";
                if (method.IsStatic)
                    methodModifiers.Add("static");
                if (method.IsAbstract)
                    methodModifiers.Add("abstract");
                var parameters = method.GetParameters();
                // add the parameters to the parameter list
                foreach (var parameter in parameters)
                {
                    var param_name = parameter.Name;
                    var param_type = parameter.ParameterType;
                    // if there's no default value for a parameter it will be null
                    object param_default_value = null;
                    if (parameter.HasDefaultValue)
                    {
                        param_default_value = parameter.DefaultValue;
                    }
                    dirty_parameter_list.Add((param_name, param_type.ToString(), param_default_value));
                }
                // correctIndicies[0] is the index of the func name, correctIndicies[1] is where the scope is located, correctIndicies[2] is where the { is 
                var correctIndicies = new int[3];
                // the index of the first occurance of the methodName in textArray
                var firstOccurance = ArrayIndex(textArray, methodName);
                while (true)
                {
                    if (firstOccurance == -1)
                    {
                        break;
                    }
                    // init the upper and lower bounds of the method to where the method name is 
                    var aboveIndex = firstOccurance;
                    var belowIndex = firstOccurance;
                    // lines above the method name
                    var linesAbove = new List<string>();
                    // lines below the method name
                    var linesBelow = new List<string>();
                    // combined list of linesAbove and linesBelow
                    var combinedLines = new List<string>();
                    // combined list turned int one line
                    var matched_line = "";
                    // check for access modifier and check above
                    while (aboveIndex >= 0)
                    {
                        // is -1 if not comment
                        var commentStatus = TextSanitizer.FindCommentType(textArray[aboveIndex].TrimStart(), commentTypes);
                        // check if the line contains the beginning of the method definition (access modifier) 
                        // need to check if it is actually a method, i.e, not a comment or statement
                        if ( commentStatus != -1 || textArray[aboveIndex].EndsWith(";") || textArray[aboveIndex].EndsWith("}"))
                        {
                            if(textArray[aboveIndex].EndsWith(";") || textArray[aboveIndex].EndsWith("}"))
                            {
                                break;
                            }
                            aboveIndex--;
                            continue;
                        }
                        else if (methodAccessMod == "private" || methodAccessMod == "default" || textArray[aboveIndex].Contains(methodAccessMod))
                        { 
                            linesAbove.Add(textArray[aboveIndex]);
                            break;
                        }
                        else if(commentStatus == -1)
                        {
                            linesAbove.Add(textArray[aboveIndex]);
                        }
                        aboveIndex--;
                    }
                    // check below for {, the end of the method definition
                    while (belowIndex < textArray.Count)
                    {
                        if (textArray[belowIndex].Contains("{"))
                        {
                            linesBelow.Add(textArray[belowIndex]);
                            break;
                        }
                        linesBelow.Add(textArray[belowIndex]);
                        belowIndex++;
                    }
                    linesAbove.RemoveAll(line => line == "");
                    // check if linesBelow or linesAbove are populated otherwise recalculate firstOccurance
                    if(linesAbove.Count == 0 || linesBelow.Count == 0)
                    {
                        firstOccurance = ArrayIndex(textArray, methodName, firstOccurance + 1);
                        continue;
                    }

                    // if there isn't a duplicate line for above and below combine
                    // the lines to add the entire method to combinedLines
                    if (linesAbove[0] != linesBelow[0])
                    {
                        combinedLines.AddRange(linesAbove);
                        combinedLines.AddRange(linesBelow);
                    }
                    else
                    {
                        combinedLines.AddRange(linesAbove);
                        linesBelow.RemoveAt(0);
                        combinedLines.AddRange(linesBelow);
                    }
                    
                    // turn the method into one string 
                    // combinedLines or matched_line can be later expanded for further analysis
                    matched_line = string.Concat(combinedLines);
                    matched_line = matched_line.Trim();

                    // find where the parameters are 
                    var open_parenthesis = matched_line.IndexOf("(", matched_line.IndexOf(methodName) + methodName.Length);
                    var close_parenthesis = matched_line.LastIndexOf(")");

                    var matched_function = true;
                    List<string> param_list = new List<string>();

                    // get the param_list iff there are parenthesis. Also perform check to see if it's the method declaration and not the call
                    if (open_parenthesis == -1 || close_parenthesis == -1)
                    {
                        matched_function = false;
                    }
                    else
                    {
                        // get part of the matched_line that contains al of the parameters, and split that by comma space
                        try
                        {
                            param_list = Regex.Split(matched_line.Substring(open_parenthesis + 1, close_parenthesis - (open_parenthesis + 1)), @"(?<=, )").Select(part => part.TrimEnd()).ToList();
                            param_list.RemoveAll((param) => param.Equals(""));
                            param_list = FixParamList(param_list);
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"This is likely not a method. Exception was: {e}\nOffending line was: {matched_line}");
                        }
                    }
                    if(param_list.Count == dirty_parameter_list.Count || TupleBreaker(dirty_parameter_list) == param_list.Count) {
                        if (TupleChecker(dirty_parameter_list))
                        {
                            param_list = FixTupleList(param_list);
                        }
                        
                        // check if the method signature matches the method we are trying to find
                        for (int i = 0; i < param_list.Count; i++)
                        {
                            // if there are no parameters in the method
                            if (param_list.Count == 1 && param_list[0] == "" )
                            {
                                break;
                            }
                            else
                            {
                                var cleanedLine = param_list[i];
                                if (param_list[i].Contains("="))
                                {
                                    cleanedLine = param_list[i].Substring(0, param_list[i].IndexOf("=")).TrimEnd();
                                }
                                //check if the types are the same
                                //split the parameter into its parts and remove empty entry in list
                                List<string> param_parts = cleanedLine.Split(new char[] { '<', '>', ' ', ')', '(', ',' }).ToList();
                                param_parts.RemoveAll(part => part == "");
                                foreach (var part in param_parts)
                                {
                                    if (!dirty_parameter_list[i].Item2.ToLower().Contains(part.ToLower()) && dirty_parameter_list[i].Item1.ToLower() != part.ToLower())
                                    {
                                        // floats are actually of type single
                                        if (dirty_parameter_list[i].Item2.ToLower().Contains("system.single") 
                                            && part.ToLower().Contains("float") && dirty_parameter_list[i].Item1.ToLower() != part.ToLower())
                                        {
                                            continue;
                                        }
                                        matched_function = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        matched_function = false;
                    }
                    if (matched_function)
                    {
                        clean_parameter_list = CreateCleanParamList(dirty_parameter_list, param_list);
                        correctIndicies[0] = firstOccurance;
                        // adjust the aboveIndex so that when getting the description it starts on the commented line
                        if (aboveIndex > 0)
                        {
                            correctIndicies[1] = aboveIndex - 1;
                        }
                        else
                        {
                            correctIndicies[1] = 0;
                        }
                        correctIndicies[2] = belowIndex;
                        break;
                    }
                    // move the firstOccurance index to the next occurance of methodName
                    firstOccurance = ArrayIndex(textArray, methodName, firstOccurance + 1);
                }
                // we've gotten the correct indicies of the correct function now.
                var description = "";
                for (int i = correctIndicies[1]; i >= 0; i--)
                {
                    Debug.Log("Correct Index Line: " + textArray[i]);
                    var line = textArray[i].Trim();
                    // break when it's no longer a comment
                    if (TextSanitizer.FindCommentType(line, commentTypes) == -1 && !(line.StartsWith("[") || line.EndsWith("]")))
                    {
                        if (TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' }) != "")
                            break;
                    }
                    // we know it's a line that looks like this: [...], so we can safely ignore it.
                    else if(line.StartsWith("[") && line.EndsWith("]"))
                    {
                        continue;
                    }
                    else
                    {
                        // keep only the text and spaces at the end of the line for markdown
                        description = textArray[i].Replace(commentTypes[TextSanitizer.FindCommentType(line, commentTypes)], "").TrimStart() + " " + description;
                    }
                }
                FunctionRepresentation function = new FunctionRepresentation(methodName, methodAccessMod, description, methodModifiers, dirty_parameter_list, clean_parameter_list);
                if (functions.ContainsKey(methodAccessMod))
                {
                    functions[methodAccessMod].Add(function);
                }
                else
                {
                    functions[methodAccessMod] = new List<FunctionRepresentation>
                    {
                        function
                    };
                }
            }
        }
        /// <summary>
        /// This checks for if there's a tuple, i.e, ```(obj, obj, obj)``` in the dirty parameter list.
        /// </summary>
        /// <param name="dirtyParameterList"></param>
        /// <returns></returns>
        private bool TupleChecker (List<(string, string, object)> dirtyParameterList)
        {
            var containsTuple = false; 
            foreach(var parameter in dirtyParameterList)
            {
                if (parameter.Item2.ToLower().Contains("tuple") && !parameter.Item2.ToLower().StartsWith("System.Collections.Generic.List".ToLower()))
                {
                    containsTuple = true;
                    break;
                }
            }
            return containsTuple;
        }
        /// <summary>
        /// This breaks tuples up to pass the paramter count check in ```ExtractFunctions()```
        /// </summary>
        /// <param name="dirtyParameterList"></param>
        /// <returns></returns>
        private int TupleBreaker(List<(string, string, object)> dirtyParameterList)
        {
            int count = 0; 
            foreach(var parameter in dirtyParameterList)
            {
                count += parameter.Item2.ToLower().Contains("tuple") ? parameter.Item2.Split(',').Length : 0;
            }
            return count;
        }
        /// <summary>
        /// Puts tuples **(only tuples, not lists or otherwise)** into the correct format for ```param_list```
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        private List<string> FixTupleList(List<string> paramList)
        {
            List<string> fixedList = new List<string>();
            int index = 0;
            while (index < paramList.Count)
            {
                // we know that if we splitted a variable such as (Obj, Obj) into "Obj," and "Obj" current index will contain ( and , but not )
                if (paramList[index].Contains("(") && paramList[index].EndsWith(","))
                {
                    // the rest of the array
                    var remainingAttributes = paramList.Skip(index).Take(paramList.Count - index).ToList();
                    var fixedString = "";
                    // check if we've added all parts of the tuple
                    while (ArrayIndex(remainingAttributes, ")") != -1 && index < paramList.Count)
                    {
                        fixedString += " " + paramList[index];
                        index++;
                        remainingAttributes.RemoveAt(0);
                    }
                    fixedList.Add(fixedString);
                }
                else
                {
                    if (paramList[index] != "")
                        fixedList.Add(paramList[index]);
                }
                index++;
            }
            return fixedList;
        }

        /// <summary>
        /// At the moment this fixes generics where the generic takes multiple parameters, e.g, ```List<T, T>```
        /// and where there are empty attributes (""). 
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        private List<string> FixParamList(List<string> paramList)
        {
            List<string> fixedList = new List<string>();
            int index = 0;
            while (index < paramList.Count)
            {
                // we know that if we splitted a variable such as List<T, T> into "List<T," and "T>" current index will contain < and , but not >
                if (paramList[index].Contains("<") && paramList[index].EndsWith(","))
                {
                    // we know that paramList[index] is something like "List<T, T>, "
                    // Note that the fast option might be less accurate
                    if (CheckBalancedBrackets(paramList[index])){
                        fixedList.Add(paramList[index]);
                        index++;
                        continue;
                    }
                    // the rest of the array
                    var remainingAttributes = paramList.Skip(index).Take(paramList.Count - index).ToList();
                    var fixedString = "";
                    // check if we've added all parts of the generic type together  
                    while (ArrayIndex(remainingAttributes, ">") != -1 && index < paramList.Count)
                    {
                        fixedString += " " + paramList[index];
                        index++;
                        remainingAttributes.RemoveAt(0);
                    }
                    fixedList.Add(fixedString);
                }
                else
                {
                    if (paramList[index] != "")
                        fixedList.Add(paramList[index]);
                }
                index++;
            }
            return fixedList;
        }
        /// <summary>
        /// Currently checks if ```<, >``` brackets are balanced
        /// </summary>
        /// <param name="checkString"></param>
        /// <returns></returns>
        private bool CheckBalancedBrackets(string checkString, bool fast = false)
        {
            if (!fast)
            {
                var stack = new List<char>();
                for (int i = 0; i < checkString.Length; i++)
                {
                    if (checkString[i] == '<')
                    {
                        stack.Add(checkString[i]);
                    }
                    else if (checkString[i] == '>')
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                }
                return stack.Count == 0;
            }
            else
            {
                return checkString.Count(c => c == '<') == checkString.Count(c => c == '>');
            }
        }
        /// <summary>
        /// Creates the ```cleanParamList``` from ```dirtyParamList``` (to get the default values) and ```paramList```
        /// </summary>
        /// <param name="dirtyParamList"></param>
        /// <param name="param_list"></param>
        /// <returns></returns>
        private List<(string, object)> CreateCleanParamList(List<(string,string,object)> dirtyParamList, List<string> paramList)
        {
            var cleanedParamList = new List<(string, object)>();
            for(int i = 0; i < paramList.Count; i++)
            {
                var typeName = paramList[i];
                var defaultVal = dirtyParamList[i].Item3;
                cleanedParamList.Add((typeName, defaultVal));
            }
            return cleanedParamList;
        }

        // ```ArrayIndex``` is a helper function used in finiding the line index of a term in an array
        private int ArrayIndex(List<string> TextArray, string Search)
        {
            for (int i = 0; i < TextArray.Count; i++)
            {
                if (TextArray[i].IndexOf(Search) != -1)
                {
                    return i;
                }
            }
            return -1;
        }
        // Overload of ```ArrayIndex```. This finds the n<sup>th</sup> occurrance of ```Search```
        private int ArrayIndex(List<string> TextArray, string Search, int index)
        {
            for (int i = 0; i < TextArray.Count; i++)
            {
                if (TextArray[i].IndexOf(Search) != -1 && i >= index)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Extracts the variables of the class, and creates ```VariableRepresentation``` objects.
        /// </summary>
        private void ExtractVariables()
        {
            Type classType = GetType(nameSpace + "." + className);
            var varInfos = classType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var textArray = File.ReadAllLines(filePath).ToList();
            var commentTypes = new string[] { "///", "//", "/*", "*", "*/" };
            foreach (var variable in varInfos)
            {
                string fieldAccess = "default";
                string dirtyFieldType = variable.FieldType.ToString();
                // the variable's anme
                string fieldName = variable.Name;
                // a list of the variable's attributes
                var attributes = variable.Attributes.ToString().Replace(" ", "").Split(',').Select(s => s.ToLowerInvariant()).ToArray();
                // if the variable is static
                var isStatic = variable.IsStatic;
                // get the access modifier
                foreach (var attribute in attributes)
                {
                    // check if the attribute is the access modifier
                    var modIndex = Array.IndexOf(accessMods, attribute);
                    Debug.Log("modIndex: " + modIndex + " attribute:" + attribute);
                    if (modIndex != -1)
                    {
                        fieldAccess = accessMods[modIndex];
                        Debug.Log("field access: " + fieldAccess);
                        break;
                    }
                }
                // get the description of the variable
                // correctIndicies[0] is the index of the func name, correctIndicies[1] is where the access modifier is located, correctIndicies[2] is where the { is 
                var correctIndicies = new int[2];
                var firstOccurance = ArrayIndex(textArray, fieldName);
                while (true)
                {
                    // conditions to break out early
                    if (firstOccurance == -1)
                    {
                        break;
                    }
                    //this condition occurs if there's a comment that contains fieldName
                    else if (textArray[firstOccurance].Trim()[0] == '/')
                    {
                        firstOccurance = ArrayIndex(textArray, fieldName, firstOccurance + 1);
                        continue;
                    }
                    var aboveIndex = firstOccurance;
                    // lines above the method name
                    var linesAbove = new List<string>();
                    // combined list turned int one line
                    var matched_line = "";
                    // check for scope and check above
                    while (aboveIndex >= 0)
                    {
                        if ((fieldAccess == "private" || fieldAccess == "default") && (TextSanitizer.FindCommentType(textArray[aboveIndex], commentTypes) != -1 || textArray[aboveIndex].EndsWith(";") || textArray[aboveIndex].EndsWith("}")))
                        {
                            aboveIndex = firstOccurance;
                            linesAbove.Add(textArray[aboveIndex]);
                            break;
                        }
                        else if (textArray[aboveIndex].Contains(fieldAccess))
                        {
                            linesAbove.Add(textArray[aboveIndex]);
                            break;
                        }
                        linesAbove.Add(textArray[aboveIndex]);
                        aboveIndex--;
                    }

                    matched_line = string.Concat(linesAbove);
                    matched_line = matched_line.Trim();

                    var nameIndex = matched_line.IndexOf(fieldName);
                    Debug.Log("matched field line: " + matched_line);

                    var matchedVariable = true;
                    //attributes from matched_line
                    string[] attrList = new string[0];

                    // get the param_list iff there are parenthesis.
                    if (nameIndex == -1)
                    {
                        matchedVariable = false;
                    }
                    else
                    {
                        try
                        {
                            // reassign attributes to space separated attributes of the variable
                            attributes = matched_line.Substring(0, nameIndex).Split(' ');
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"This is likely not a variable. Exception was: {e}\nOffending line was: {matched_line}");
                        }
                    }

                    for (int i = 0; i < attrList.Length; i++)
                    {

                        // check if param_list is larger than parameter_list
                        if (i >= attrList.Length)
                        {
                            matchedVariable = false;
                            break;
                        }

                        //check if the types are the same
                        //split the parameter into its parts and remove empty entries
                        List<string> attr_parts = attrList[i].Split(new char[] { '<', '>', ' ' }).ToList();
                        attr_parts.Remove("");
                        Debug.Log("attr_parts: "+ attr_parts);
                        foreach (var part in attr_parts)
                        {

                            if (!attributes.Contains(part.ToLower()))
                            {
                                matchedVariable = false;
                                break;
                            }

                        }

                    }
                    if (matchedVariable)
                    {
                        correctIndicies[0] = firstOccurance;
                        // adjust the aboveIndex so that when getting the description it starts on the commented line
                        if (aboveIndex > 0)
                        {
                            correctIndicies[1] = aboveIndex - 1;
                        }
                        else
                        {
                            correctIndicies[1] = 0;
                        }
                        break;
                    }
                    firstOccurance = ArrayIndex(textArray, fieldName, firstOccurance + 1);
                }
                // we've gotten the correct indicies of the correct function now, which lets us get the description of the variable
                var description = "";
                for (int i = correctIndicies[1]; i >= 0; i--)
                {
                    var line = textArray[i].Trim();
                    // break when it's no longer a comment or a blank line
                    if (TextSanitizer.FindCommentType(line, commentTypes) == -1 && !(line.StartsWith("[") || line.EndsWith("]")))
                    {
                        if (TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' }) != "")
                            break;
                    }
                    // we know it's a line that looks like this: [...], so we can safely ignore it.
                    else if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        continue;
                    }
                    else
                    {
                        description = line.Replace(commentTypes[TextSanitizer.FindCommentType(line, commentTypes)], "").TrimStart() + " " + description;
                    }
                }

                // generate the fixed list of variables
                var cleanedAttributes = FixVariableAttributes(attributes);

                // create and add a VariableRepresentation to the variables dictionary
                VariableRepresentation addVar = new VariableRepresentation(fieldName, dirtyFieldType, cleanedAttributes[cleanedAttributes.Count-1], p_acessMod: fieldAccess, p_description: description, p_modifiers: cleanedAttributes);
                if (variables.ContainsKey(fieldAccess))
                {
                    variables[fieldAccess].Add(addVar);
                }
                else
                {
                    variables[fieldAccess] = new List<VariableRepresentation>
                    {
                        addVar
                    };
                }

            }
        }
        /// <summary>
        /// At the moment fixes generics where the generic takes multiple parameters, e.g, ```List<T, T>```
        /// and where there are empty attributes ("")
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private List<string> FixVariableAttributes(string[] attributes)
        {
            List<string> fixedList = new List<string>();
            int index = 0;
            while(index < attributes.Length)
            {
                // we know that if we splitted a variable such as List<T, T> into "List<T," and "T>" current index will contain < and , but not >
                if(attributes[index].Contains("<") && attributes[index].EndsWith(","))
                {
                    // the rest of the array
                    var remainingAttributes = attributes.Skip(index).Take(attributes.Length - index).ToList();
                    var fixedString = "";
                    // check if we've added all parts of the generic type together  
                    while (ArrayIndex(remainingAttributes, ">") != -1 && index < attributes.Length) 
                    {
                        fixedString += " " + attributes[index];
                        index++;
                        remainingAttributes.RemoveAt(0);
                    }
                    fixedList.Add(fixedString);
                }
                else
                {
                    if(attributes[index] != "")
                        fixedList.Add(attributes[index]);
                }
                index++;
            }
            return fixedList;
        }
        /// <summary>
        /// Extracts the imports of the class
        /// </summary>
        private void ExtractImports()
        {
            while (true)
            {
                // remove semicolons, tabs, and spaces so that we the format: usinglibrary where library is a the imported library
                string line = fileReader.ReadLine();
                if (line != null)
                {
                    line = TextSanitizer.RemoveCharacters(line, new char[] { '\t', ';', ' ' });
                }

                if (!line.StartsWith("using"))
                {
                    fileReader.ReverseLine();
                    break;
                }
                else
                {
                    imports.Add(line.Substring(5));
                }
            }
        }
        /// <summary>
        /// Extracts information about a class: the description, class name, access modifier, and what it inherits
        /// </summary>
        private void ExtractClassInformation()
        {
            var inComment = false;
            var comments = new List<string>();
            var currentComment = "";
            while (true)
            {
                string line = fileReader.ReadLine();
                if (line != null)
                {
                    line = line.Trim();
                }
                else
                {
                    return;
                }
                // extracts the class description is there is one
                if (line.StartsWith("///") || line.StartsWith("//") || line.StartsWith("/*") || (line.StartsWith("*") && inComment) || line.StartsWith("*/"))
                {

                    inComment = true;
                    if (line.StartsWith("///"))
                        currentComment += line.Replace("///", "");
                    else if (line.StartsWith("//"))
                        currentComment += line.Replace("//", "");
                    else if (line.StartsWith("/*"))
                        currentComment += line.Replace("/*", "");
                    else if (line.StartsWith("*"))
                        currentComment += line.Replace("*", "");
                    else if (line.StartsWith("*/"))
                        currentComment += line.Replace("*/", "");
                    continue;
                }
                // if the previous line was a comment then inComment will still be true
                if (inComment)
                {
                    inComment = false;
                    comments.Add(currentComment);
                }

                if (line.Contains("class") || line.Contains("interface") || line.Contains("abstract") || line.Contains("struct"))
                {
                    accessModifier = line.Substring(0, line.IndexOf("class")).Replace(" ", "");
                    className = ClassNameHelper(TextSanitizer.RemoveCharacters(line, new char[] { ' ' })).Trim();
                    inheritance = InheritanceHelper(line);
                    declaration = line;
                    if (comments.Count > 0)
                        description = comments[comments.Count - 1];
                    break;
                } else if (line.Contains("namespace"))
                {
                    nameSpace = line.Substring(line.IndexOf("namespace") + "namespace".Length).Trim(new char[] { ' ', '{' });
                }
            }
        }
        /// <summary>
        /// A helper method for detecting what a class inherits
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<string> InheritanceHelper(string line)
        {
            if (line.Contains(":"))
            {
                dynamic filteredString = line.Substring(line.IndexOf(":") + 1);
                filteredString = filteredString.Split(new char[] { ',' });
                return new List<string>(filteredString);
            }
            else
            {
                var notFoundList = new List<string>();
                notFoundList.Add("NO INHERITANCE");
                return notFoundList;
            }

        }
        /// <summary>
        /// A helper method for detecting the class's name
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string ClassNameHelper(string line)
        {
            if (line.Contains(":"))
            {
                return line.Substring(line.IndexOf("class") + 5, line.IndexOf(":") - (line.IndexOf("class") + 5)).Trim(new char[] { ' ' });
            }
            else if (line.Contains("class"))
            {
                return line.Substring(line.IndexOf("class") + 5);
            }
            else if (line.Contains("interface"))
            {
                isInterface = true;
                return line.Substring(line.IndexOf("interface") + "interface".Length);
            }
            else if (line.Contains("abstract"))
            {
                isAbstract = true;
                return line.Substring(line.IndexOf("abstract") + "abstract".Length);
            }
            else if (line.Contains("struct"))
            {
                isAbstract = true;
                return line.Substring(line.IndexOf("struct") + "struct".Length);
            }
            else
            {
                return "NO CLASS NAME DETECTED";
            }
        }

        /// <summary>
        /// A wrapper for ```Type.GetType()``` that works with different assemblies\n\n
        /// https://stackoverflow.com/a/11811046/2793618
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
