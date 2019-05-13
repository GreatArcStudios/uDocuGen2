﻿using System.Collections.Generic;
using System.IO;

namespace uDocumentGenerator.helpers
{
    /// <summary>
    /// Provides a list of c# files that should be analyzed. 
    /// </summary>
    public class ProjectTree
    {
        public string folderPath;
        public FileTree filesList;
        public ProjectTree(string fPath)
        {
            folderPath = fPath;
            var cleandedList = TextSanitizer.RemoveApplicationPath(GenerateFileList(folderPath));
            filesList = new FileTree(cleandedList);
        }
        /// <summary>
        /// Recursively generate the list of c# files 
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        private List<string> GenerateFileList(string fPath)
        {
            List<string> subList = new List<string>();
            if (Directory.GetFiles(fPath, "*.cs").Length == 0 && Directory.GetDirectories(fPath).Length == 0)
            {
                return subList;
            }
            else if (Directory.GetDirectories(fPath).Length == 0)
            {
                subList = FindcsharpFiles(fPath);
                return subList;
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
            return subList;
        }
        private List<string> FindcsharpFiles(string fPath)
        {
            List<string> filesList = new List<string>();
            filesList.AddRange(Directory.GetFiles(fPath, "*.cs"));
            return filesList;
        }
    }
}

