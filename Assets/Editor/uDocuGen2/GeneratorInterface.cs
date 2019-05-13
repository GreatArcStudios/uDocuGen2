using UnityEditor;

namespace uDocumentGenerator.ui
{
    public class GeneratorInterface : EditorWindow
    {
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
        }

    }
}
