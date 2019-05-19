using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// The tree is formatted as follows: 
    ///     1. a file will have no children (a leaf), i.e, subDir.Length == 0
    ///     2. a branch will have it's own value and will not end in .cs with non-empty subDir list
    /// </summary>
    public class FileTree
    {
        public List<FileTree> subDir = new List<FileTree>();
        public string currentDirectory;
        public List<string> filePath;
        public FileTree(List<string> fPath)
        {
            filePath = fPath;
            BuildTree();
            Debug.Log(ToString());
        }
        public FileTree(string subDirectoryPath)
        {
            filePath = new List<string>();
            filePath.Add(subDirectoryPath);
            BuildTree();
        }
        private void BuildTree()
        {
            foreach (string fp in filePath)
            {
                // if there are more branches to construct
                if (Regex.Matches(fp, @"\\[^\\]*\\").Count > 1)
                {
                    var match = Regex.Match(fp, @"\\[^\\]*\\");
                    //Debug.Log(matches);
                    if (match.Success)
                    {
                        var matchedDirectory = match.Value.Substring(1, match.Value.Length - 2);
                        FileTree subDirectory;

                        // if the current directory matches the matched directory check for more common subdirectories
                        // then add the new subDirectory into the subDir of the lastCommonDirectory FileTree
                        if (currentDirectory == matchedDirectory)
                        {
                            // insert a FileTree in the subdir of lastCommomDirectory
                            FileTree lastCommonDirectory = this;
                            // this will have the file path that isn't common with the rest
                            string addPath = fp;
                            var nextFolder = Regex.Match(addPath.Substring(currentDirectory.Length + 1), @"\\[^\\]*\\");
                            var containsNextStep = false;
                            while (containsNextStep)
                            {
                                //check if subtrees have next step of the path 
                                for (var i = 0; i < lastCommonDirectory.subDir.Count; i++)
                                {
                                    // substr of nextFolder gets rid of slashes
                                    if (lastCommonDirectory.subDir[i].currentDirectory == nextFolder.Value.Substring(1, nextFolder.Value.Length - 2))
                                    {
                                        containsNextStep = true;
                                        lastCommonDirectory = lastCommonDirectory.subDir[i];
                                        addPath = addPath.Substring(nextFolder.Length + 1);
                                        nextFolder = Regex.Match(addPath.Substring(nextFolder.Length + 1), @"\\[^\\]*\\");
                                        break;
                                    }
                                }
                                // we know that there were no more commonalities in that step 
                                if (!containsNextStep)
                                {
                                    break;
                                }
                            }
                            subDirectory = new FileTree(addPath);
                            lastCommonDirectory.subDir.Add(subDirectory);
                        }
                        else
                        {
                            currentDirectory = matchedDirectory;
                            subDirectory = new FileTree(fp.Substring(currentDirectory.Length + 1));
                        }
                    }
                }
                else
                {
                    //return when we reach a file
                    currentDirectory = fp.Substring(1);
                    return;
                }
            }

        }
        private string strFormated(int depth = 0) 
        {
            var formatted_str = "";
            if (subDir.Count == 0)
            {
                return " ";
            }
            else
            {
                formatted_str = string.Concat(Enumerable.Repeat(" ", depth)) + $"{currentDirectory}\n"; 
                foreach(var subDirectroy in subDir)
                {
                    formatted_str += subDirectroy.strFormated(depth++);
                }
            }
            return formatted_str;
        }
        public override string ToString()
        {

            return strFormated();
        }
    }
}
