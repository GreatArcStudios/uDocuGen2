using System.IO;

namespace uDocumentGenerator.Helpers
{
    public class FileReader
    {
        private string[] fileText;
        private int index = 0;
        public FileReader(string filePath)
        {
            fileText = File.ReadAllLines(filePath);
        }
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
        public void ResetPosition()
        {
            index = 0;
        }

    }
}
