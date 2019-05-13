using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uDocumentGenerator.helpers
{
    /// <summary>
    /// The tree is formatted as follows: 
    ///     1. a file will have no children (a leaf), i.e, subDir.Length == 0
    ///     2. a branch will have it's own value and will not end in .cs with non-empty subDir list
    /// </summary>
    public class FileTree
    {
        public List<List<string>> subDir = new List<List<string>>();
        public List<string> currentDirectory = new List<string>();
        public List<string> filePath;
        public FileTree(List<string> fPath)
        {
            filePath = fPath;
        }
        private void buildTree()
        {
             
        }
    }
}
