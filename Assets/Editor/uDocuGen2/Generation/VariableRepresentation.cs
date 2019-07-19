using System.Collections.Generic;
namespace uDocumentGenerator.Generation
{
    /// <summary>
    /// The important information extracted from a variable, represented as an object.
    /// </summary>
    public class VariableRepresentation
    {
        // A list of all attributes not covered by the other fields
        List<string> attributes = new List<string>();
        // The description of the variable
        public string description;
        // The access modifier of the variable
        public string accessMod;
        // The variable anme
        public string varName;
        // The type returned by reflection - use this if cleanType doesn't give you good results for your variables
        public string dirtyType;
        // Type determined by ExtractVariables
        public string cleanType;

        public VariableRepresentation(string p_name, string p_dirtyType, string p_cleanType, string p_acessMod, string p_description, List<string> p_modifiers)
        {
            accessMod = p_acessMod;
            description = p_description;
            attributes = p_modifiers;
            varName = p_name;
            dirtyType = p_dirtyType;
            cleanType = p_cleanType;
        }
        /// <summary>
        /// Method to manually append a description to a ```VariableRepresentation```
        /// </summary>
        /// <param name="description"></param>
        public void AppendDescription(string description)
        {
            this.description += description;
        }
        /// <summary>
        /// Method to manually append an attribute to a ```VariableRepresentation```
        /// </summary>
        /// <param name="attribute"></param>
        public void AddAttribute(string attribute)
        {
            attributes.Add(attribute);
        }
        /// <summary>
        /// Method to manually remove an attribute from a ```VariableRepresentation```
        /// </summary>
        /// <param name="attribute"></param>
        public void RemoveAttribute(string attribute)
        {
            attributes.Remove(attribute);
        }
    }
}
