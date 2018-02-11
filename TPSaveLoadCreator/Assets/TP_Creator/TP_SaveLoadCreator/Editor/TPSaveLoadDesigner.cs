using UnityEngine;
using UnityEditor;
using TP.SaveLoad;
using UnityEditor.SceneManagement;
using TP.Utilities;

namespace TP.SaveLoadEditor
{
    [InitializeOnLoad]
    public class TPSaveLoadDesigner : EditorWindow
    {
        public static TPSaveLoadDesigner window;
        static string currentScene;

        [MenuItem("TP_Creator/TP_SaveLoadCreator")]
        public static void OpenWindow()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.Log("You can't change SaveLoad Manager Designer runtime!");
                return;
            }
            window = (TPSaveLoadDesigner)GetWindow(typeof(TPSaveLoadDesigner));
            currentScene = EditorSceneManager.GetActiveScene().name;
            EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
            window.minSize = new Vector2(615, 290);
            window.maxSize = new Vector2(615, 290);
            window.Show();
        }

        static void hierarchyWindowChanged()
        {
            if (currentScene != EditorSceneManager.GetActiveScene().name)
            {
                if (TPSaveLoadToolsWindow.window)
                    TPSaveLoadToolsWindow.window.Close();
                if (window)
                    window.Close();
            }
        }

        public static TPEditorGUIData EditorData;
        public static TPSaveLoadCreator SaveLoadCreator;
        public static GUISkin skin;

        Texture2D headerTexture;
        Texture2D managerTexture;
        Texture2D toolTexture;

        Rect headerSection;
        Rect managerSection;
        Rect toolSection;

        bool existManager;
        bool toggleChange;

        public static SerializedObject creator;

        void OnEnable()
        {
            InitEditorData();
            InitTextures();
            InitCreator();

            if(SaveLoadCreator)
                creator = new SerializedObject(SaveLoadCreator);
        }

        void InitEditorData()
        {
            string path = "Assets/TP_Creator/_CreatorResources/";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            EditorData = AssetDatabase.LoadAssetAtPath(
                   path + "SaveLoadEditorGUIData.asset",
                   typeof(TPEditorGUIData)) as TPEditorGUIData;
            
            if (EditorData == null)
                CreateEditorData();
            else
                CheckGUIData();

            skin = EditorData.GUISkin;
        }

        void CheckGUIData()
        {
            if (EditorData.GUISkin == null)
                EditorData.GUISkin = AssetDatabase.LoadAssetAtPath(
                      "Assets/TP_Creator/_CreatorResources/TPEditorGUISkin.guiskin",
                      typeof(GUISkin)) as GUISkin;

            EditorUtility.SetDirty(EditorData);
        }

        void CreateEditorData()
        {
            TPEditorGUIData newEditorData = ScriptableObject.CreateInstance<TPEditorGUIData>();
            AssetDatabase.CreateAsset(newEditorData, "Assets/TP_Creator/_CreatorResources/SaveLoadEditorGUIData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorData = newEditorData;
            CheckGUIData();
        }

        void InitTextures()
        {
            Color colorHeader = new Color(0.19f, 0.19f, 0.19f);
            Color color = new Color(0.15f, 0.15f, 0.15f);

            headerTexture = new Texture2D(1, 1);
            headerTexture.SetPixel(0, 0, colorHeader);
            headerTexture.Apply();

            managerTexture = new Texture2D(1, 1);
            managerTexture.SetPixel(0, 0, color);
            managerTexture.Apply();

            toolTexture = new Texture2D(1, 1);
            toolTexture.SetPixel(0, 0, color);
            toolTexture.Apply();
        }

        static void InitCreator()
        {
            if (SaveLoadCreator == null)
            {
                SaveLoadCreator = FindObjectOfType<TPSaveLoadCreator>();

                if (SaveLoadCreator != null)
                    UpdateManager();
            }
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (TPSaveLoadToolsWindow.window)
                    TPSaveLoadToolsWindow.window.Close();
                this.Close();
            }
            DrawLayouts();
            DrawHeader();
            DrawManager();
            DrawTools();
        }

        void DrawLayouts()
        {
            headerSection = new Rect(0, 0, Screen.width, 50);
            managerSection = new Rect(0, 50, Screen.width / 2, Screen.height);
            toolSection = new Rect(Screen.width / 2, 50, Screen.width / 2, Screen.height);

            GUI.DrawTexture(headerSection, headerTexture);
            GUI.DrawTexture(managerSection, managerTexture);
            GUI.DrawTexture(toolSection, toolTexture);
        }

        void DrawHeader()
        {
            GUILayout.BeginArea(headerSection);
            GUILayout.Label("TP SaveLoad Creator - Manage your Persistance!", skin.GetStyle("HeaderLabel"));
            GUILayout.EndArea();
        }

        void DrawManager()
        {
            GUILayout.BeginArea(managerSection);
            GUILayout.Label("SaveLoad Manager - Core", skin.box);

            if (SaveLoadCreator == null)
            {
                InitializeManager();
            }
            else
            {
                ToggleDebugMode();
                ResetManager();

                if (GUILayout.Button("Refresh and update", skin.button, GUILayout.Height(70)))
                {
                    UpdateManager();
                }
            }

            GUILayout.EndArea();
        }

        void InitializeManager()
        {
            if (GUILayout.Button("Initialize New Manager", skin.button, GUILayout.Height(60)))
            {
                GameObject go = (new GameObject("TP_SaveLoadManager", typeof(TPSaveLoadCreator)));
                SaveLoadCreator = go.GetComponent<TPSaveLoadCreator>();
                UpdateManager();
                Debug.Log("Save Load Manager created!");
            }

            if (GUILayout.Button("Initialize Exist Manager", skin.button, GUILayout.Height(60)))
                existManager = !existManager;

            if (existManager)
                SaveLoadCreator = EditorGUILayout.ObjectField(SaveLoadCreator, typeof(TPSaveLoadCreator), true,
                    GUILayout.Height(30)) as TPSaveLoadCreator;
        }

        void ResetManager()
        {
            if (GUILayout.Button("Reset Manager", skin.button, GUILayout.Height(45)))
                SaveLoadCreator = null;
        }

        void ToggleDebugMode()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle Debug Mode", skin.button, GUILayout.Height(40)))
            {
                TPSaveLoadCreator.DebugMode = !TPSaveLoadCreator.DebugMode;
                if (TPSaveLoadToolsWindow.window)
                {
                    UpdateManager();
                    TPSaveLoadToolsWindow.window.Close();
                }
                if (SaveLoadCreator)
                {
                    MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>();
                    foreach (var mono in monos)
                    {
                        var item = mono.GetComponent<TPSaveLoadPersistance>();
                        if (item != null)
                            item.Refresh();
                    }
                }
            }
            GUILayout.Toggle(TPSaveLoadCreator.DebugMode, GUIContent.none, GUILayout.Width(15));
            GUILayout.EndHorizontal();
        }

        public static void UpdateManager()
        {
            if (SaveLoadCreator)
            {
                //SaveLoadCreator.Refresh();
                //SaveLoadCreator.OnValidate();
                EditorUtility.SetDirty(SaveLoadCreator);
            }

            if (SaveLoadCreator)
                creator = new SerializedObject(SaveLoadCreator);

            if (creator != null)
            if (creator.targetObject != null)
            {
                creator.UpdateIfRequiredOrScript();
                creator.ApplyModifiedProperties();
            }
        }

        void DrawTools()
        {

            GUILayout.BeginArea(toolSection);
            GUILayout.Label("SaveLoad Manager - Tools", skin.box);

            if (SaveLoadCreator == null)
            {
                GUILayout.EndArea();
                return;
            }

            if (GUILayout.Button("JSON Persistance", skin.button, GUILayout.Height(60)))
            {
                TPSaveLoadToolsWindow.OpenToolWindow(TPSaveLoadToolsWindow.Tool.JSON);
            }
            if (GUILayout.Button("XML Persistance", skin.button, GUILayout.Height(60)))
            {
                TPSaveLoadToolsWindow.OpenToolWindow(TPSaveLoadToolsWindow.Tool.XML);
            }
            if (GUILayout.Button("Binary Persistance", skin.button, GUILayout.Height(60)))
            {
                TPSaveLoadToolsWindow.OpenToolWindow(TPSaveLoadToolsWindow.Tool.Binary);
            }
            GUILayout.EndArea();
        }

    }
}