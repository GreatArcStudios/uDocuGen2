using UnityEditor;
using UnityEngine;
using uDocumentGenerator;
using System.Collections.Generic;
using uDocumentGenerator.Helpers;
using uDocumentGenerator.Generation;

namespace uDocumentGenerator.UI
{
    /// <summary>
    /// The UI interface used to display generation params and generation button
    /// </summary>
    public class GeneratorInterface : EditorWindow
    {
        // Button to generate the JSON
        public string generateButton = "Generate";
        //The version of the project. By default it is assigned to ```Application.version```
        public string version;
        // The user's name. By default it is assigned to ```Application.companyName```
        public string authorName;
        // The project name. By default it is assigned to ```Application.productName```
        public string projectName; 

        // Label next to project directory chooser button
        public GUIContent ProjPathLabel = new GUIContent("Choose project directory");     
        
        // Label next to website directory chooser button
        public GUIContent WebsitePathLabel = new GUIContent("Directory to react project");

        // Label next to acknowledgement directory chooser button
        public GUIContent acknowledgementLabel = new GUIContent("Path to markdown file");   
        
        // Label next to description directory chooser button
        public GUIContent descriptionLabel = new GUIContent("Path to markdown file");

        // Label next to getting started directory chooser button
        public GUIContent gettingStartedLabel = new GUIContent("Path to markdown file");

        // The file path for the project
        string projectFilePath = "";

        // The file path for the website root folder, i.e, where build, src, public, and node modules folders are located
        string websiteFilePath = "";     
        
        // The file path for the acknowledgements file
        string acknowledgementFilePath = "";

        // The file path for the description file
        string descriptionFilePath = "";

        // The file path for the description file
        string gettingStartedFilePath = "";

        // The acknowledgements file as a ```string```
        // \n Initalize it to ```""``` so that the app can run with out a specified file.
        string acknowledgements = "";

        // The description file as a ```string```
        // \n Initalize it to ```""``` so that the app can run with out a specified file.
        string description = ""; 
        
        // The getting started file as a ```string```
        // \n Initalize it to ```""``` so that the app can run with out a specified file.
        string gettingStarted = "";

        // Array Index is used in finiding the line index of a term in an array
        private int ArrayIndex(List<string> TextArray, string Search)
        {
            for (int i = 0; i < TextArray.Count; i++)
            {
                if (TextArray[i].IndexOf(Search) != -1)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// What happens when the window is shown.
        /// </summary>
        [MenuItem("Tools/Generate Documentation")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GeneratorInterface));
        }
        /// <summary>
        /// When the UI is enabled
        /// </summary>
        void OnEnable()
        {
            titleContent = new GUIContent("uDocumentGenerator");
            minSize = new Vector2(250, 350);
            ProjPathLabel = new GUIContent("Choose a Directory");
            WebsitePathLabel = new GUIContent("Choose a Directory");
            projectName = Application.productName;
            authorName = Application.companyName;
            version = Application.version;
        }
        /// <summary>
        /// What to do when the GUI is shown
        /// </summary>
        void OnGUI()
        {
            GUILayout.Label("uDocumentGenerator", EditorStyles.boldLabel);
            GUILayout.Label("Version: 1.0.0", EditorStyles.label);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Find me: ");
            if (GUILayout.Button("Website"))
                Help.BrowseURL("https://ezhu.build");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Project file path:", EditorStyles.boldLabel);
            if (EditorGUILayout.DropdownButton(ProjPathLabel, FocusType.Keyboard))
            {
                projectFilePath = EditorUtility.OpenFolderPanel("Choose project file path", Application.dataPath, "Choose folder...");
                ProjPathLabel = new GUIContent(projectFilePath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Website template path:", EditorStyles.boldLabel);
            if (EditorGUILayout.DropdownButton(WebsitePathLabel, FocusType.Keyboard))
            {
                websiteFilePath = EditorUtility.SaveFolderPanel("Choose website project file path", Application.dataPath, projectName);
                WebsitePathLabel = new GUIContent(websiteFilePath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            GUILayout.Label("Project Name: ", EditorStyles.boldLabel);
            projectName = GUILayout.TextField(projectName);

            GUILayout.Label("Author Name: ", EditorStyles.boldLabel);
            authorName = GUILayout.TextField(authorName);

            GUILayout.Label("Version: ", EditorStyles.boldLabel);
            version = GUILayout.TextField(version);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Acknowledgement file:", EditorStyles.boldLabel);
            if (EditorGUILayout.DropdownButton(acknowledgementLabel, FocusType.Keyboard))
            {
                acknowledgementFilePath = EditorUtility.OpenFilePanelWithFilters("Choose acknowledgement file", Application.dataPath, new string[] { "Markdown", "md", "Text files","txt", "Any" ,"*"});
                acknowledgementLabel = new GUIContent(acknowledgementFilePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Description file:", EditorStyles.boldLabel);
            if (EditorGUILayout.DropdownButton(descriptionLabel, FocusType.Keyboard))
            {
                descriptionFilePath = EditorUtility.OpenFilePanelWithFilters("Choose description file", Application.dataPath, new string[] { "Markdown", "md", "Text files", "txt", "Any" ,"*" });
                descriptionLabel = new GUIContent(descriptionFilePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Getting started file:", EditorStyles.boldLabel);
            if (EditorGUILayout.DropdownButton(gettingStartedLabel, FocusType.Keyboard))
            {
                gettingStartedFilePath = EditorUtility.OpenFilePanelWithFilters("Choose getting started file", Application.dataPath, new string[] { "Markdown", "md", "Text files", "txt", "Any", "*" });
                gettingStartedLabel = new GUIContent(gettingStartedFilePath);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            if (GUILayout.Button(generateButton))
            {
                List<string> exclusions = new List<string>();
                exclusions.Add(TextSanitizer.AppPath + "\\" + "Tests");
                acknowledgements = $"`{new FileReader(acknowledgementFilePath).ToString().Replace("`", "\\`")}`";
                description = $"`{new FileReader(descriptionFilePath).ToString().Replace("`", "\\`")}`";
                gettingStarted = $"`{new FileReader(gettingStartedFilePath).ToString().Replace("`", "\\`")}`";
                List<string> authorInfo = new List<string>();
                authorInfo.Add($"\"projectName\": \"{projectName}\"");
                authorInfo.Add($"\"authorName\": \"{authorName}\"");
                authorInfo.Add($"\"version\": \"{version}\"");
                DocGen.Generate(projectFilePath, websiteFilePath, exclusions);
                DocGen.AppendUserInfo(acknowledgements, description, authorInfo, websiteFilePath, gettingStarted);
            }
            EditorGUILayout.Separator();

            GUILayout.Label("© Eric Zhu 2019");
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();


        }
    }
}
