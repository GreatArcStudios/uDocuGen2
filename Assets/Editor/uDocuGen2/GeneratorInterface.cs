using UnityEditor;
using UnityEngine;
using uDocumentGenerator;

namespace uDocumentGenerator.UI
{
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
        void OnEnable()
        {
        }
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
                Generation.DocGen.Generate(filePath);
            }
        }

    }
}
