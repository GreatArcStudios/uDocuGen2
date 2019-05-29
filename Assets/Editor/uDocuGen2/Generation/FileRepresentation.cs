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
    /// This class represents a file, which is used to generate documentation
    /// </summary>
    public class FileRepresentation
    {
        public List<string> imports = new List<string>();
        // keys are access modifiers and values are nested lists of variable names
        public Dictionary<string, List<List<string>>> variables = new Dictionary<string, List<List<string>>>();
        public List<string> inheritance = new List<string>();
        public string scope;
        public string className;
        public string description;
        public string filePath;
        public string nameSpace = "";
        // values are formatted: functions[key][0] is a list of strings containing param type and name as an element, functions[key][1] is the function name, functions[key][2] is a list of other modifiers
        public Dictionary<string, List<FunctionRepresentation>> functions = new Dictionary<string, List<FunctionRepresentation>>();
        private readonly FileReader streamReader;
        private readonly string[] accessMods = new string[] { "public", "protected", "private", "internal", "protected internal", "private protected" };
        private bool isInterface = false;
        private bool isAbstract = false;
        public FileRepresentation(string fp)
        {
            filePath = fp;
            streamReader = new FileReader(fp);
            ExtractImports();
            ExtractClassInformation();
            ExtractVariables();
            ExtractFunctions();
        }
        private void ExtractFunctions()
        {

            Type classType = Type.GetType(nameSpace + "." + className);
            var methodInfos = classType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var textArray = File.ReadAllLines(filePath).ToList();
            var commentTypes = new string[] { "///", "//", "/*", "*", "*/" };

            foreach (var method in methodInfos)
            {
                string methodScope = "default";
                string name = method.Name;
                var modifiers = new List<string>();
                var parameter_list = new List<(string, string, object)>();
                if (method.IsPublic)
                    methodScope = "public";
                else if (method.IsPrivate)
                    methodScope = "private";
                else if (method.IsAssembly)
                    methodScope = "internal";
                else if (method.IsFamily)
                    methodScope = "protected";
                if (method.IsStatic)
                    modifiers.Add("static");
                if (method.IsAbstract)
                    modifiers.Add("abstract");
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
                    parameter_list.Add((param_name, param_type.ToString(), param_default_value));
                }
                // correctIndicies[0] is the index of the func name, correctIndicies[1] is where the scope is located, correctIndicies[2] is where the { is 
                var correctIndicies = new int[3];
                var firstOccurance = ArrayIndex(textArray, name);
                while (true)
                {
                    if (firstOccurance == -1)
                    {
                        break;
                    }
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
                    // check for scope and check above
                    while (aboveIndex >= 0)
                    {

                        if (methodScope == "private" && (TextSanitizer.FindCommentType(textArray[aboveIndex], commentTypes) != -1 || textArray[aboveIndex].EndsWith(";") || textArray[aboveIndex].EndsWith("}")))
                        {
                            aboveIndex = firstOccurance;
                            linesAbove.Add(textArray[aboveIndex]);
                            break;
                        }
                        else if (textArray[aboveIndex].Contains(methodScope))
                        {
                            linesAbove.Add(textArray[aboveIndex]);
                            break;
                        }
                        linesAbove.Add(textArray[aboveIndex]);
                        aboveIndex--;
                    }
                    // check below for {
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

                    foreach (var line in combinedLines)
                    {
                        matched_line += line;
                    }
                    matched_line = matched_line.Trim();
                    var open_parenthesis = matched_line.IndexOf("(");
                    var close_parenthesis = matched_line.IndexOf(")");
                    Debug.Log(matched_line);
                    var matched_function = true;
                    string[] param_list = new string[0];

                    // get the param_list iff there are parenthesis.
                    if (open_parenthesis == -1 || close_parenthesis == -1)
                    {
                        matched_function = false;
                    }
                    else
                    {
                        try
                        {
                            param_list = matched_line.Substring(open_parenthesis + 1, close_parenthesis - (open_parenthesis + 1)).Split(',');
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"This was likely not a method. Exception was: {e}\nOffending line was: {matched_line}");
                        }
                    }

                    for (int i = 0; i < param_list.Length; i++)
                    {
                        if (param_list.Length == 1 && param_list[0] == "")
                        {
                            break;
                        }
                        else
                        {
                            //try
                            //{
                            //    Type param_type = Type.GetType(param_list[i].Split(' ')[0]);
                            //    if (!(parameter_list[i].Item2.ToLower() == param_type.ToString().ToLower()))
                            //    {
                            //        matched_function = false;
                            //        break;
                            //    }
                            //}
                            //catch (Exception e)
                            //{
                            //    Debug.Log($"This parameter was likely not a valid type (not a parameter). Exception: {e}");
                            //    matched_function = false;
                            //    break;
                            //}
                            // check if param_list is larger than parameter_list
                            if (i >= parameter_list.Count)
                            {
                                matched_function = false;
                                break;
                            }


                            //check if the types are the same
                            //split the parameter into its parts and remove empty entry in list
                            List<string> param_parts = param_list[i].Split(new char[] { '<', '>', ' ' }).ToList();
                            param_parts.Remove("");
                            foreach (var part in param_parts)
                            {
                                if (!parameter_list[i].Item2.ToLower().Contains(part.ToLower()) && !(parameter_list[i].Item1.ToLower() == part.ToLower()))
                                {
                                    matched_function = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (matched_function)
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
                        correctIndicies[2] = belowIndex;
                        break;
                    }
                    firstOccurance = ArrayIndex(textArray, name, firstOccurance + 1);
                }
                // we've gotten the correct indicies of the correct function now.
                var description = "";
                for (int i = correctIndicies[1]; i >= 0; i--)
                {
                    // break when it's no longer a comment and a blank line
                    if (TextSanitizer.FindCommentType(textArray[i].Trim(), commentTypes) == -1)
                    {
                        if(TextSanitizer.RemoveCharacters(textArray[i], new char[] { '\t', ' ' }) != "")
                            break;
                    }
                    else
                    {
                        description = textArray[i].Replace(commentTypes[TextSanitizer.FindCommentType(textArray[i].Trim(), commentTypes)], "").Trim() + " " + description;
                    }
                }
                FunctionRepresentation function = new FunctionRepresentation(methodScope, description, modifiers, parameter_list, name);
                if (functions.ContainsKey(methodScope))
                {
                    functions[methodScope].Add(function);
                }
                else
                {
                    functions[methodScope] = new List<FunctionRepresentation>();
                    functions[methodScope].Add(function);
                }
            }
        }
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
        private void ExtractVariables()
        {

            // start reading from the top of the file
            streamReader.ResetPosition();
            string line = streamReader.ReadLine();
            bool inFunction = false;
            var bracketcount = 0;
            while ((line = streamReader.ReadLine()) != null)
            {
                // get rid of tabs and spaces
                line = line.TrimStart(new char[] { '\t' });
                //determine if it is a variable
                //we know it is a statement or variable if there's a semicolon.
                bracketcount += line.Count(c => c == '{');
                if (bracketcount <= 2 && nameSpace != "")
                {
                    // we know that before the constructor we will have at most 2 brackets with variables placed before the constructor
                    if (line.EndsWith(";") && !line.StartsWith("using"))
                    {
                        // extract the varible from the line.
                        VarHelper(line);
                    }
                }
                else
                {
                    // we know we've entered a function if a line contains a { but not a ;
                    if (line.EndsWith("{") && !line.EndsWith(";"))
                    {
                        inFunction = true;
                    }
                    // we know we've exited a function if a line contains a } but not a ;
                    // continue onto the next iteration 
                    else if (line.EndsWith("}") && !line.EndsWith(";"))
                    {
                        inFunction = false;
                        continue;
                    }
                    else if (!inFunction && line.EndsWith(";"))
                    {
                        // extract the varible from the line.

                        VarHelper(line);
                    }
                }

            }
        }
        private void VarHelper(string line)
        {
            //separate the parts of the line by spaces
            var matches = Regex.Matches(line, @"[^ ]*").OfType<Match>().Select(m => m.Value).ToArray();
            foreach (var match in matches)
            {
                // is a variable and not a statement, e.g, if statement
                if (!isAbstract && !isInterface && Array.IndexOf(matches, ";") != -1)
                {
                    var variableList = new List<string>();

                    if (Array.IndexOf(accessMods, matches[0]) != -1)
                    {
                        if (matches.Contains("="))
                        {
                            for (int i = 1; i < Array.IndexOf(matches, "="); i++)
                            {
                                variableList.Add(matches[i]);
                            }
                        }
                        else
                        {
                            for (int i = 1; i < matches.Length - 1; i++)
                            {
                                variableList.Add(matches[i]);
                            }
                        }
                        if (variables.ContainsKey(matches[0]))
                        {
                            variables[matches[0]].Add(variableList);
                        }
                        else
                        {
                            variables[matches[0]] = new List<List<string>>
                            {
                                variableList
                            };
                        }
                    }
                    else
                    {
                        if (matches.Contains("="))
                        {
                            for (int i = 0; i < Array.IndexOf(matches, "="); i++)
                            {
                                variableList.Add(matches[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < matches.Length - 1; i++)
                            {
                                variableList.Add(matches[i]);
                            }
                        }
                        if (variables.ContainsKey("default"))
                        {
                            variables["default"].Add(variableList);
                        }
                        else
                        {
                            variables["default"] = new List<List<string>>
                            {
                                variableList
                            };
                        }
                    }
                }
            }
        }
        private void ExtractImports()
        {
            while (true)
            {
                // remove semicolons, tabs, and spaces so that we the format: usinglibrary where library is a the imported library
                string line = streamReader.ReadLine();
                if (line != null)
                {
                    line = TextSanitizer.RemoveCharacters(line, new char[] { '\t', ';', ' ' });
                }

                if (!line.StartsWith("using"))
                {
                    streamReader.ReverseLine();
                    break;
                }
                else
                {
                    imports.Add(line.Substring(5));
                }
            }
        }
        private void ExtractClassInformation()
        {
            var inComment = false;
            var comments = new List<string>();
            var currentComment = "";
            while (true)
            {
                string line = streamReader.ReadLine();
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
                        currentComment += line.Replace("///", "").Replace("<summary>", "");
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
                    scope = line.Substring(0, line.IndexOf("class")).Replace(" ", "");
                    className = ClassNameHelper(TextSanitizer.RemoveCharacters(line, new char[] { ' ' })).Trim();
                    inheritance = InheritanceHelper(line);
                    if (comments.Count > 0)
                        description = comments[comments.Count - 1];
                    break;
                }

                else if (line.Contains("namespace"))
                {
                    nameSpace = line.Substring(line.IndexOf("namespace") + "namespace".Length).Trim(new char[] { ' ', '{' });
                }
            }
        }

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
    }
}
