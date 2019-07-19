using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// Contains static methods for various text/string operations
    /// </summary>
    public class TextSanitizer
    {

        // The application path
        private static readonly string appPath = Application.dataPath.Replace(@"/", "\\");

        /// <summary>
        /// The property for AppPath
        /// </summary>
        public static string AppPath => appPath;

        /// <summary>
        /// Removes the application data path from the string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveApplicationPath(string path)
        {
            return path.Replace(appPath, "");
        }
        /// <summary>
        /// Overload for RemoveApplication path. Takes a list of paths instead of just a path.
        /// </summary>
        /// <param name="stringList"></param>
        /// <returns></returns>
        public static List<string> RemoveApplicationPath(List<string> stringList)
        {
            List<string> cleanedList = new List<string>();            
            foreach (var item in stringList)
            {
                cleanedList.Add(RemoveApplicationPath(item));
            }
            return cleanedList;
        }
        /// <summary>
        /// Reverses the slashes in file paths to keep them consistent
        /// </summary>
        /// <param name="filePaths"></param>
        public static void ReverseSlashes(List<string> filePaths)
        {
            for (int i = 0; i < filePaths.Count; i++)
            {
                var fixedPath = filePaths[i].Replace(@"/", "\\");
                filePaths[i] = fixedPath;
            }
        }

        public static List<string> RemoveCommonDirectory(List<string> target, List<string> toRemove)
        {
            List<string> cleandedList = new List<string>();
            for(int i = 0; i < toRemove.Count; i++)
            {
                for(int j = 0; j<target.Count; j++)
                {
                    if (!target[j].StartsWith(toRemove[i]))
                    {
                        cleandedList.Add(target[j]);
                    }
                }
            }
            return cleandedList;
        }

        /// <summary>
        /// Removes characters specified from a string. Used in cleaning a line from a file. Non-descructive. 
        /// </summary>
        /// <param name="toProcess"></param>
        /// <param name="removeChars"></param>
        /// <returns></returns>
        public static string RemoveCharacters(string toProcess, char[] removeChars)
        {
            var cleanedString = toProcess;
            foreach(var character in removeChars)
            {
                cleanedString = cleanedString.Replace(character.ToString(), "");
            }
            return cleanedString;

        }
        /// <summary>
        /// Finds the comment type from a list of: ```{ "///", "//", "/*", "*", "*/" }```. Returns -1 iff no matches
        /// </summary>
        /// <param name="line"></param>
        /// <param name="searches"></param>
        /// <returns></returns>
        public static int FindCommentType(string line, string[] searches)
        {
            for (int i = 0; i < searches.Length; i++)
            {
                if (line.StartsWith(searches[i]))
                {
                    return i;
                }
            }
            return -1; 
        }
    }
}

