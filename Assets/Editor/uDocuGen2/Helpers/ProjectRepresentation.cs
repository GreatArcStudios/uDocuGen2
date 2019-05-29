using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// Provides a representation of a project
    /// </summary>
    public class ProjectRepresentation
    {
        public string folderPath;
        public FileTree fileTree;
        public List<string> fileList;
        public List<string> directoryExclusionsList;
        public ProjectRepresentation(string fPath, List<string> exclusions)
        {
            folderPath = fPath;
            var cleandedList = TextSanitizer.RemoveApplicationPath(GenerateFileList(folderPath));
            directoryExclusionsList = TextSanitizer.RemoveApplicationPath(exclusions);
            TextSanitizer.ReverseSlashes(directoryExclusionsList);
            cleandedList = TextSanitizer.RemoveCommonDirectory(cleandedList, directoryExclusionsList);
            Debug.Log(string.Join(",", cleandedList.ToArray()));
            fileList = cleandedList;
            fileTree = new FileTree(cleandedList, TextSanitizer.AppPath);

        }
        /// <summary>
        /// Recursively generate the flattened list of c# files paths
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        private List<string> GenerateFileList(string fPath)
        {
            List<string> subList = new List<string>();
            // if the directory has no more subdirectories and has no c# files
            if (Directory.GetFiles(fPath, "*.cs").Length == 0 && Directory.GetDirectories(fPath).Length == 0)
            {
                return subList;
            }
            // if the directory has c# files but no more subdirectories
            else if (Directory.GetDirectories(fPath).Length == 0)
            {
                subList = FindcsharpFiles(fPath);
            }
            else
            {
                // add files in current file path
                subList.AddRange(FindcsharpFiles(fPath));
                // add files in sub directories
                var subDirectories = Directory.GetDirectories(fPath);
                foreach (var sd in subDirectories)
                {
                    var returnedList = GenerateFileList(sd);
                    subList.AddRange(returnedList);
                }
            }
            TextSanitizer.ReverseSlashes(subList);
            return subList;
        }
        /// <summary>
        /// Helper method to find the c# files in a directory
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        private List<string> FindcsharpFiles(string fPath)
        {
            List<string> filesList = new List<string>();
            filesList.AddRange(Directory.GetFiles(fPath, "*.cs"));
            return filesList;
        }
            
    }
}

