using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using uDocumentGenerator.Helpers;
using UnityEngine;

namespace uDocumentGenerator.Conversion
{       
    /// <summary>
    /// Uses ```Newtonsoft.Json``` to turn the generated ```projectRepresentation``` into JSON
    /// </summary>
    public class JsonConverter
    {
        // The project representation 
        ProjectRepresentation projectRepresentation;

        public JsonConverter(ProjectRepresentation project)
        {
            projectRepresentation = project;

        }
        /// <summary>
        /// Serialize and save the project as JSON in the specified ```savePath``` 
        /// </summary>
        /// <param name="savePath"></param>
        public void ConvertSave(String savePath)
        {
            File.WriteAllText(savePath+"//config.js", "const config = " + Convert());
        }
        /// <summary>
        /// Converts the projectRepresentation using settings into JSON
        /// </summary>
        /// <returns></returns>
        public string Convert()
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new MyContractResolver() };
            //DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            //dcr.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
            //settings.ContractResolver = dcr;

            return JsonConvert.SerializeObject(projectRepresentation, Formatting.Indented, settings: settings);
        }

    }
    /// <summary>
    /// Settings for JSON converter
    /// </summary>
    class MyContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo[] fields = objectType.GetFields(flags);
            return fields
                .Concat(objectType.GetProperties(flags).Where(propInfo => propInfo.CanWrite))
                .ToList();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, MemberSerialization.Fields);
        }

    }
}
