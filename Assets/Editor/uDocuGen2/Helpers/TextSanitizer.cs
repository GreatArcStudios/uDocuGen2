using System.Collections.Generic;
using UnityEngine; 

namespace uDocumentGenerator.helpers
{
    /// <summary>
    /// Contains static methods for various text/string operations
    /// </summary>
    public class TextSanitizer
    {
        private static string appPath = Application.dataPath;
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

    }
}

