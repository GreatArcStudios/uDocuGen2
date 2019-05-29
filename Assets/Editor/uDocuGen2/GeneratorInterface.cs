using UnityEditor;
using UnityEngine;
using uDocumentGenerator;
using System.Collections.Generic;
using uDocumentGenerator.Helpers;

namespace uDocumentGenerator.UI
{
    /// <summary>
    /// test0
    /// </summary>
    public class GeneratorInterface : EditorWindow
    {
        // ui variables
        public string generateButton = "Generate";
        public GUIContent ButtonLabel = new GUIContent("Choose a Directory");

        //other variables
        string filePath = "";

        [MenuItem("Tools/Generate Documentation")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GeneratorInterface));
        }
        /// <summary>
        /// test1
        /// </summary>
        void OnEnable()
        {
        }
        // test2
        void OnGUI()
        {
            if (EditorGUILayout.DropdownButton(ButtonLabel, FocusType.Keyboard))
            {
                filePath = EditorUtility.OpenFolderPanel("Choose Where Documentation is Saved", Application.dataPath, "test");
                ButtonLabel = new GUIContent(filePath);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            if (GUILayout.Button(generateButton)){
                List<string> exclusions = new List<string>();
                exclusions.Add(TextSanitizer.AppPath + "\\" + "Tests");
                Generation.DocGen.Generate(filePath, exclusions);
            }
        }

    }
}
