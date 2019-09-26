using System.IO;

namespace uDocumentGenerator.Helpers
{
    /// <summary>
    /// Reads the contents of the file at the specified ```filePath```. <br/>
    /// Contains many useful methods that enhances the usability of ```File.ReadAllLines()```
    /// </summary>
    public class FileReader
    {
        // The list where each element is a line of the file
        private string[] fileText;
        // The line we are reading from in ```fileText```
        private int index = 0;
        // The file represented as a string
        private string fileString;
        public FileReader(string filePath)
        {
            filePath = TextSanitizer.ReverseSlashes(filePath);
            fileText = File.ReadAllLines(filePath);
            fileString = File.ReadAllText(filePath);
        }
        /// <summary>
        /// Read the next line of the file
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            if (index < fileText.Length)
            {
                var return_line = fileText[index];
                index++;
                return return_line;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Read the previous line if it is not an invalid index
        /// </summary>
        /// <returns></returns>
        public string ReverseLine()
        {
            if (index > 0)
            {
                var return_line = fileText[index];
                index--;
                return return_line;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Go back to the beginning of the file
        /// </summary>
        public void ResetPosition()
        {
            index = 0;
        }
        /// <summary>
        /// Resets the file and ```index```
        /// </summary>
        public void Reset()
        {
            fileText = null;
            index = 0;
        }

        /// <summary>
        /// Overrides the parent ```ToString()```
        /// </summary>
        /// <returns></returns>
        new public string ToString()
        {
            return fileString;
        }

    }
}
