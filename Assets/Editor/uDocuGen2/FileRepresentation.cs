using System;
using System.Collections.Generic;
using System.IO;

namespace uDocumentGenerator.Generation
{
    /// <summary>
    /// This class represents a file, which is used to generate documentation
    /// </summary>
    public class FileRepresentation
    {
        public List<string> imports;
        public List<string> variables;
        public List<string> inheritance;
        public string scope;
        public string className;
        public string description;
        public string filePath;
        public Dictionary<string, string> functions;
        private StreamReader steamReader; 
        public FileRepresentation(string fp)
        {
            filePath = fp;
            steamReader = new StreamReader(fp);
            ExtractImports();
            ExtractClassInformation();
            ExtractFunctions();
        }

        private void ExtractFunctions()
        {
            

        }
        private void ExtractVariables()
        {
            
        }

        private void ExtractImports()
        {
            while (true){
                // remove semicolons, tabs, and spaces so that we the format: usinglibrary where library is a the imported library
                string line = Helpers.TextSanitizer.RemoveCharacters(steamReader.ReadLine(), new char[] { '\t', ';', ' ' });
                if (!line.Contains("using")){
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
            while (true)
            {
                string line = Helpers.TextSanitizer.RemoveCharacters(steamReader.ReadLine(), new char[] { '\t', ' '});
                if (line.Contains("class"))
                {
                    scope = line.Substring(0, line.IndexOf("class"));
                    className = ClassNameHelper(line);
                    inheritance = InheritanceHelper(line);
                    break;
                }
            }
       }

        private List<string> InheritanceHelper(string line)
        {
            dynamic filteredString = line.Substring(line.IndexOf(":"));
            filteredString = filteredString.Split(new char[] { ',' });
            return new List<string>(filteredString);
        }

        private string ClassNameHelper(string line)
        {
            if (line.Contains(":"))
            {
                return line.Substring(line.IndexOf("class") + 5, line.IndexOf(":"));
            }
            else
            {
                return line.Substring(line.IndexOf("class") + 5);
            }
        }
    }
}
