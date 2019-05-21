using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace uDocumentGenerator.Generation
{
    /// <summary>
    /// This class represents a file, which is used to generate documentation
    /// </summary>
    public class FileRepresentation
    {
        public List<string> imports;
        public Dictionary<string, List<List<string>>> variables;
        public List<string> inheritance;
        public string scope;
        public string className;
        public string description;
        public string filePath;
        public string nameSpace = "";
        // values are formatted: functions[key][0] is a list of strings containing param type and name as an element, functions[key][1] is the function name, functions[key][2] is a list of other modifiers
        public Dictionary<string, List<FunctionRepresentation>> functions;
        private readonly StreamReader streamReader;
        private readonly string[] accessMods = new string[] { "public", "protected", "private", "internal", "protected internal", "private protected" };
        private bool isInterface = false;
        private bool isAbstract = false;
        public FileRepresentation(string fp)
        {
            filePath = fp;
            streamReader = new StreamReader(fp);
            ExtractImports();
            ExtractClassInformation();
            ExtractVariables();
            ExtractFunctions();
        }
        /// <summary>
        /// 
        /// </summary>
        //private void ExtractFunctions()
        //{
        //    streamReader.DiscardBufferedData();
        //    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        //    string line = streamReader.ReadLine();
        //    var inComment = false;
        //    var comments = new List<(string, string)>();
        //    var currentComment = "";
        //    var nextLines = "";
        //    while (true)
        //    {
        //        line = streamReader.ReadLine();
        //        if (line != null)
        //        {
        //            line = Helpers.TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' });

        //        }
        //        else
        //        {
        //            return;
        //        }

        //        if (line.StartsWith("///") || line.StartsWith("//") || line.StartsWith("/*"))
        //        {
        //            if (line.StartsWith("/*"))
        //            {
        //                inComment = true;
        //                while (!line.StartsWith("*/"))
        //                {
        //                    currentComment += line.Replace("*", "");
        //                    line = streamReader.ReadLine();
        //                    if (line != null)
        //                    {
        //                        line = Helpers.TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' });
        //                    }
        //                    else
        //                    {
        //                        return;

        //                    }
        //                }
        //                // find the next non whitespace line 
        //                Regex regex = new Regex(@"\s+");
        //                while (true)
        //                {
        //                    line = regex.Replace(line, "");
        //                    if (line.Length > 0)
        //                    {
        //                        break;
        //                    }
        //                    line = streamReader.ReadLine();
        //                    if (line != null)
        //                    {
        //                        line = Helpers.TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' });
        //                    }
        //                    else
        //                    {
        //                        return;

        //                    }
        //                }
        //                VerifyFunction(line);
        //                inComment = false;
        //            }
        //            else
        //            {
        //                inComment = true;
        //                currentComment += line.Replace("///", "").Replace("<summary>", "");
        //            }

        //        }
        //        else if (inComment)
        //        {
        //            inComment = false;
        //            var nextNonblankLine = "";
        //            while (true)
        //            {
        //                if ()
        //                {

        //                }
        //            }
        //            comments.Add((currentComment, ));
        //        }
        //    }
        //}
        //private void VerifyFunction(string line)
        //{

        //}
        private void ExtractFunctions()
        {

            Type classType = Type.GetType(nameSpace + "." + className);
            var methodInfos = classType.GetMethods();
            var textArray = File.ReadAllLines(filePath);
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
                foreach(var parameter in parameters)
                {
                    var param_name = parameter.Name;
                    var param_type = parameter.ParameterType;
                    object param_default_value = null;
                    if (parameter.HasDefaultValue)
                    {
                        param_default_value = parameter.DefaultValue;
                    }
                    parameter_list.Add((param_name, param_type.ToString(), param_default_value));
                } 
                
            }
        }

        private void ExtractVariables()
        {
            streamReader.DiscardBufferedData();
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string line = streamReader.ReadLine();
            bool inFunction = false;
            var bracketcount = 0;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.TrimStart(new char[] { '\t' });
                //determine if it is a variable
                //we know it is a statement or variable if there's a semicolon.
                bracketcount += line.Count(c => c == '{');
                if (bracketcount <= 2 && nameSpace != "")
                {
                    // we know that before the constructor we will have at most 2 brackets with variables placed before the constructor
                    if (line.Contains(";"))
                    {
                        VarHelper(line);
                    }
                }
                else
                {
                    // we know we've entered a function if a line contains a { but not a ;
                    if (line.Contains("{") && !line.Contains(";"))
                    {
                        inFunction = true;
                    }
                    // we know we've exited a function if a line contains a } but not a ;
                    else if (line.Contains("}") && !line.Contains(";"))
                    {
                        inFunction = false;
                        continue;
                    }
                    else if (!inFunction && line.Contains(";"))
                    {
                        VarHelper(line);
                    }
                }

            }
        }
        private void VarHelper(string line)
        {
            //separate the parts of the line by spaces
            var matches = Regex.Matches(line, @"[^ ]").OfType<Match>().Select(m => m.Value).ToArray();
            foreach (var match in matches)
            {
                // is a variable and not a 
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
                    line = Helpers.TextSanitizer.RemoveCharacters(line, new char[] { '\t', ';', ' ' });
                }

                if (!line.Contains("using"))
                {
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
                    line = Helpers.TextSanitizer.RemoveCharacters(line, new char[] { '\t', ' ' });
                }
                else
                {
                    return;
                }
                // extracts the class description is there is one
                if (line.StartsWith("///") || line.StartsWith("//"))
                {
                    inComment = true;
                    currentComment += line.Replace("///", "").Replace("<summary>", "");
                }
                // if the previous line was a comment then inComment will still be true
                else if (inComment)
                {
                    inComment = false;
                    comments.Add(currentComment);
                }

                if (line.Contains("class") || line.Contains("interface") || line.Contains("abstract") || line.Contains("struct"))
                {
                    scope = line.Substring(0, line.IndexOf("class")).Replace(" ", "");
                    className = ClassNameHelper(line);
                    inheritance = InheritanceHelper(line);
                    description = comments[comments.Count - 1];
                    break;
                }

                if (line.Contains("namespace"))
                {
                    nameSpace = line.Substring(line.IndexOf("namespace") + "namespace".Length).Trim(new char[] { ' ', '{' });
                }
            }
        }

        private List<string> InheritanceHelper(string line)
        {
            if (line.Contains(":"))
            {
                dynamic filteredString = line.Substring(line.IndexOf(":"));
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
                return line.Substring(line.IndexOf("class") + 5, line.IndexOf(":") - (line.IndexOf("class") + 6)).Trim(new char[] { ' ' });
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
