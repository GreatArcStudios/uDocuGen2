﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// Contains static methods for various text/string operations
    /// </summary>
    public class TextSanitizer
    {
        private static string appPath = Application.dataPath.Replace(@"/", "\\");
        /// <summary>
        /// Removes the application data path from the string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveApplicationPath(string path)
        {
            return path.Replace(appPath, "");
        }
        public static List<string> RemoveApplicationPath(List<string> stringList)
        {
            List<string> cleanedList = new List<string>();            
            foreach (var item in stringList)
            {
                cleanedList.Add(RemoveApplicationPath(item));
            }
            return cleanedList;
        }
        public static void ReverseSlashes(List<string> filePaths)
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                var fixedPath = filePaths[i].Replace(@"/", "\\");
                filePaths[i] = fixedPath;
            }
        }
        public static string RemoveCharacters(string toProcess, char[] removeChars)
        {
            var cleanedString = toProcess;
            foreach(var character in removeChars)
            {
                cleanedString = cleanedString.Replace(character.ToString(), "");
            }
            return cleanedString;

        }
    }
}

