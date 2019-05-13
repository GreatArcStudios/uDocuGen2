using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uDocumentGenerator.helpers
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
        }
        public FileTree(string subDirectoryPath)
        {
            filePath.Add(subDirectoryPath);
        }
        private void BuildTree()
        {
            foreach(string fp in filePath)
            {
                // if there are more branches to construct
                if (fp.Contains("\\"))
                {
                    var matches = Regex.Match(fp, "\\[^\\]*\\");
                    if (matches.Success)
                    {
                        currentDirectory = matches.Value.Substring(1,matches.Value.Length-2);
                        FileTree subDirectory = new FileTree(fp.Substring(currentDirectory.Length+1));
                        bool existInSubDir = false; 
                        for (int i = 0; i < subDir.Count; i++)
                        {
                            if(subDir[i].currentDirectory == subDirectory.currentDirectory)
                            {
                                subDir[i].subDir.AddRange(subDirectory.subDir);
                                existInSubDir = true;
                                break; 
                            }
                        }
                        if (!existInSubDir)
                        {
                            subDir.Add(subDirectory);
                        }
                    }
                }
                else
                {
                    //return when we reach a file
                    currentDirectory = fp;
                    return;
                }
            }
            
        }
    }
}
