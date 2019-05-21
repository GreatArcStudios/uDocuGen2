using System;
using System.Collections.Generic;
namespace uDocumentGenerator.Generation
{
    public class FunctionRepresentation
    {
        // each param is formatted as name, paramtype, and default value
        List<(string, string, object)> paramList = new List<(string, string, object)>();
        List<string> modifiers = new List<string>();
        string description;
        string scope;
        string func_name;

        public string Func_name { get => func_name; set => func_name = value; }
        public string Scope { get => scope; set => scope = value; }
        public string Description { get => description; set => description = value; }

        public FunctionRepresentation(string p_scope, string p_description, List<string> p_modifiers, List<(string, string, object)> p_param_list, string p_name)
        {
            Scope = p_scope;
            Description = p_description;
            modifiers = p_modifiers;
            paramList = p_param_list;
            Func_name = p_name;
        }
        public void Addparameter((string, string, object) parameter)
        {
            paramList.Add(parameter);
        }
        public void Addmodifier(string modifier)
        {
            modifiers.Add(modifier);
        }

    }
}
