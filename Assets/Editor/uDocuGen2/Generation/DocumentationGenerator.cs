
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uDocumentGenerator.Conversion;
using uDocumentGenerator.Helpers;
using UnityEngine;

namespace uDocumentGenerator.Generation
{  
    /// <summary>
    /// Generates a ```ProjectRepresentation``` of the specified project and corresponding JSON file  
    /// </summary>
    public class DocGen
    {
        /// <summary>
        /// The ```ProjectRepresentation``` that will be generated
        /// </summary>
        public static ProjectRepresentation projectTree;
        /// <summary>
        /// Generates the documentation by creating a JsonConverter with projectTree as a parameter
        /// </summary>
        /// <param name="genFilePath"></param>
        /// <param name="exclusions"></param>
        public static void Generate(string genFilePath, string saveFilePath, List<string> exclusions)
        {
            projectTree = new ProjectRepresentation(genFilePath, exclusions);
            JsonConverter jsonConverter = new JsonConverter(projectTree);
            jsonConverter.ConvertSave(saveFilePath);
        }
        /// <summary>
        /// Appends user information like acknowledgements, project description, and author name to ```config.js```
        /// </summary>
        /// <param name="acknowledgements"></param>
        /// <param name="projDescription"></param>
        /// <param name="authorInfo"></param>
        /// <param name="savePath"></param>
        public static void AppendUserInfo(string acknowledgements, string projDescription, List<string> authorInfo, string savePath)
        {
            List<string> config = File.ReadAllLines(savePath + "//config.js").ToList();
            config[config.Count - 1] += ";";
            config.Add("const acknowledgements = " + acknowledgements + ";");
            config.Add("const description = " +  projDescription + ";");
            string userInfo = "const userInformation = {";
            foreach(string info in authorInfo)
            {
                userInfo += info + ",";
            }
            userInfo += "};";
            config.Add(userInfo);
            File.WriteAllText(savePath + "//config.js", String.Join(String.Empty, config));
        }
    }

}
