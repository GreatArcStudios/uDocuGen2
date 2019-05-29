
using System.Collections.Generic;

namespace uDocumentGenerator.Generation
{  
    /// <summary>
    /// Generates a JSON file  
    /// </summary>
    public class DocGen
    {
        public static Helpers.ProjectRepresentation projectTree;
        
        public static void Generate(string filePath, List<string> exclusions)
        {
            
            projectTree = new Helpers.ProjectRepresentation(filePath, exclusions);
        }
    }

}
