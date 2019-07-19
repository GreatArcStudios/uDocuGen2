using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using uDocumentGenerator.Generation;
using UnityEngine;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// The tree is formatted as follows: 
    /// <ul> 
    ///     <li> 
    ///         a file will have no children (a leaf), i.e, ```subDir.Length == 0```
    ///     </li>
    ///     <li>
    ///         a branch will have it's own value and will not end in .cs with non-empty subDir list
    ///     </li>
    /// </ul>
    /// </summary>
    public class FileTree
    {
        // If this is not a leaf, this will not be null -> provides the recursive structure of FileTree
        public List<FileTree> subDir = new List<FileTree>();
        // The file path to the current directory
        public string currentDirectory;
        // The file paths of the files in the subDirs/current level
        public List<string> filePaths;
        // The parents & ancestors of this "file level" 
        private string prevDirectory;
        // If this ```FileTree``` is a leaf, it will be populated with a ```FileRepresentation```
        private FileRepresentation fileRepresentation;
        /// <summary>
        /// This constructor is used for instantiating a FileTree
        /// </summary>
        /// <param name="fPath"></param>
        /// <param name="previousDirectory"></param>
        public FileTree(List<string> fPath, string previousDirectory)
        {
            filePaths = fPath;
            prevDirectory = previousDirectory;
            BuildTree();
            Console.WriteLine(ToString());
            Debug.Log(ToString());
        }
        /// <summary>
        /// This constructor is used in recursive calls in BuildTree
        /// </summary>
        /// <param name="subDirectoryPath"></param>
        /// <param name="previousDirectory"></param>
        public FileTree(string subDirectoryPath, string previousDirectory)
        {
            filePaths = new List<string>();
            filePaths.Add(subDirectoryPath);
            prevDirectory = previousDirectory;
            BuildTree();
        }
        /// <summary>
        /// Builds the FileTree -> contains all the fileReps.  
        /// This is a recursive data structure.
        /// </summary>
        private void BuildTree()
        {
            foreach (string fp in filePaths)
            {
                // if there are more branches to construct
                if (Regex.Matches(fp, @"\\[^\\]*[^\\]").Count > 1)
                {
                    var match = Regex.Match(fp, @"\\[^\\]*[^\\]");
                    //Debug.Log(matches);
                    if (match.Success)
                    {
                        var matchedDirectory = match.Value.Substring(1, match.Value.Length - 1);
                        FileTree subDirectory;

                        // if the current directory matches the matched directory check for more common subdirectories
                        // then add the new subDirectory into the subDir of this FileTree
                        // this will only execute if there are more than one items in filePath
                        if (currentDirectory == matchedDirectory)
                        {
                            // insert a FileTree in the subdir of lastCommomDirectory
                            FileTree lastCommonDirectory = this;
                            // this will have the file path that isn't common with the rest
                            string addPath = fp.Substring(currentDirectory.Length + 1);
                            var nextFolder = Regex.Match(addPath, @"\\[^\\]*[^\\]");
                            var containsNextStep = true;
                            var allCommonDirectories = new List<string>();
                            while (containsNextStep)
                            {
                                containsNextStep = false;
                                //check if subtrees have next step of the path 
                                for (var i = 0; i < lastCommonDirectory.subDir.Count; i++)
                                {
                                    // substr of nextFolder gets rid of slashes
                                    if (lastCommonDirectory.subDir[i].currentDirectory == nextFolder.Value.Substring(1))
                                    {
                                        containsNextStep = true;

                                        allCommonDirectories.Add(lastCommonDirectory.currentDirectory);

                                        lastCommonDirectory = lastCommonDirectory.subDir[i];

                                        addPath = addPath.Substring(nextFolder.Length);
                                        nextFolder = Regex.Match(addPath, @"\\[^\\]*[^\\]");

                                        break;
                                    }
                                }
                                // we know that there were no more commonalities in that step 
                                if (!containsNextStep)
                                {
                                    break;
                                }
                            }
                            var commonPath = BuildCommonPath(allCommonDirectories) + "\\" + lastCommonDirectory.currentDirectory;
                            Debug.Log(prevDirectory + "\\" + commonPath);
                            subDirectory = new FileTree(addPath, prevDirectory + "\\" + commonPath);
                            lastCommonDirectory.subDir.Add(subDirectory);
                        }
                        else
                        {
                            currentDirectory = matchedDirectory;
                            subDirectory = new FileTree(fp.Substring(currentDirectory.Length + 1), prevDirectory + "\\" + currentDirectory);
                            subDir.Add(subDirectory);
                        }
                    }
                }
                else
                {
                    //return when we reach a file
                    currentDirectory = fp.Substring(1);
                    fileRepresentation = new Generation.FileRepresentation(prevDirectory + "\\" + currentDirectory);
                    return;
                }
            }

        }
        /// <summary>
        /// Builds a common file path for directories.
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        private string BuildCommonPath(List<string> directories)
        {
            var path = "";
            for (int i = 0; i < directories.Count; i++)
            {
                string directory = directories[i];
                if (i > 0)
                    path += $"\\{directory}";
                else
                    path += $"{directory}";
            }
            return path;
        }
        /// <summary>
        /// Returns a formatted string of the file tree
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private string StrFormated(int depth)
        {
            var formatted_str = "";
            if (subDir.Count == 0)
            {
                return new string(' ', --depth * 5) + currentDirectory + "\n";
            }
            else
            {
                formatted_str = new string(' ', depth * 5) + $"{currentDirectory}\n";
                var newDepth = ++depth;
                foreach (var subDirectroy in subDir)
                {
                    formatted_str += subDirectroy.StrFormated(newDepth);
                }
            }
            return formatted_str;
        }
        /// <summary>
        /// Overrides the default ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return StrFormated(0);
        }
        /// <summary>
        /// This will return the FileRepresentation of a file if it exists. Otherwise it will return null. The file path should be split among slashes with the last element being the file.cs
        /// </summary>
        /// <param name="pathList"></param>
        /// <returns></returns>
        public FileRepresentation FindFile(List<string> pathList)
        {
            FileRepresentation target = null;
            // when we should reach the file
            if (pathList.Count == 1)
            {
                if (pathList[0] != currentDirectory)
                {
                    return null;
                }
                else
                {
                    return fileRepresentation;
                }
            }
            else if (pathList[0] != currentDirectory)
            {
                return null;
            }
            else
            {
                var nextFolder = pathList[1];
                pathList.RemoveAt(0);
                foreach (var directory in subDir)
                {
                    if (directory.currentDirectory == nextFolder)
                    {
                        var result = FindFile(pathList);
                        if (result != null)
                        {
                            target = result;
                            return target;
                        }
                    }
                }
            }
            return target;
        }
    }
}
