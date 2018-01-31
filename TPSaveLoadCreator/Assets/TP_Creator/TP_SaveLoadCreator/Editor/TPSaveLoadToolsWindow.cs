using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TP_SaveLoadEditor
{
    [InitializeOnLoad]
    internal class TPSaveLoadToolsWindow : EditorWindow
    {
        public static TPSaveLoadToolsWindow window;
        public enum Tool
        {
            JSON,
            XML,
            Binary
        }
        public static Tool tool;

        SerializedProperty _JSONPersistance;
        SerializedProperty _XMLPersistance;
        SerializedProperty _BinaryPersistance;

        SerializedProperty j_UseApplicationPath;
        SerializedProperty j_SaveName;
        SerializedProperty j_ExtensionName;
        SerializedProperty j_CustomPath;

        SerializedProperty x_UseApplicationPath;
        SerializedProperty x_SaveName;
        SerializedProperty x_ExtensionName;
        SerializedProperty x_CustomPath;

        SerializedProperty b_UseApplicationPath;
        SerializedProperty b_SaveName;
        SerializedProperty b_ExtensionName;
        SerializedProperty b_CustomPath;

        GUIContent content0 = new GUIContent("Application Path");
        GUIContent content1 = new GUIContent("Save File Name");
        GUIContent content2 = new GUIContent("Extension Name");
        GUIContent content3 = new GUIContent("Custom Application Path");
        
        Texture2D mainTexture;
        Vector2 scrollPos = Vector2.zero;
        Vector2 textureVec;

        Rect mainRect;

        static float windowSize = 515;
        static string currentScene;

        public static void OpenToolWindow(Tool _tool)
        {
            if (window != null)
                window.Close();

            window = (TPSaveLoadToolsWindow)GetWindow(typeof(TPSaveLoadToolsWindow));

            currentScene = EditorSceneManager.GetActiveScene().name;
            EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;

            window.minSize = new Vector2(windowSize, windowSize);
            window.maxSize = new Vector2(windowSize, windowSize);
            window.Show();
            tool = _tool;
            AssetDatabase.OpenAsset(TPSaveLoadDesigner.SaveLoadCreator);
        }

        static void hierarchyWindowChanged()
        {
            if (currentScene != EditorSceneManager.GetActiveScene().name)
            {
                if (TPSaveLoadDesigner.window)
                    TPSaveLoadDesigner.window.Close();
                if (window)
                    window.Close();
            }
        }

        void OnEnable()
        {
            InitTextures();

            FindLayoutProperties();
        }

        void FindLayoutProperties()
        {
            _JSONPersistance = TPSaveLoadDesigner.creator.FindProperty("JSONMethod");
            _XMLPersistance = TPSaveLoadDesigner.creator.FindProperty("XMLMethod");
            _BinaryPersistance = TPSaveLoadDesigner.creator.FindProperty("BinaryMethod");

            j_UseApplicationPath = _JSONPersistance.FindPropertyRelative("UseApplicationPath");
            j_SaveName = _JSONPersistance.FindPropertyRelative("SaveName");
            j_ExtensionName = _JSONPersistance.FindPropertyRelative("ExtensionName");
            j_CustomPath = _JSONPersistance.FindPropertyRelative("CustomPath");

            x_UseApplicationPath = _XMLPersistance.FindPropertyRelative("UseApplicationPath");
            x_SaveName = _XMLPersistance.FindPropertyRelative("SaveName");
            x_ExtensionName = _XMLPersistance.FindPropertyRelative("ExtensionName");
            x_CustomPath = _XMLPersistance.FindPropertyRelative("CustomPath");

            b_UseApplicationPath = _BinaryPersistance.FindPropertyRelative("UseApplicationPath");
            b_SaveName = _BinaryPersistance.FindPropertyRelative("SaveName");
            b_ExtensionName = _BinaryPersistance.FindPropertyRelative("ExtensionName");
            b_CustomPath = _BinaryPersistance.FindPropertyRelative("CustomPath");
        }

        void InitTextures()
        {
            Color color = new Color(0.19f, 0.19f, 0.19f);
            mainTexture = new Texture2D(1, 1);
            mainTexture.SetPixel(0, 0, color);
            mainTexture.Apply();
        }

        void OnGUI()
        {
            mainRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(mainRect, mainTexture);
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            DrawTool();
            GUILayout.EndScrollView();
        }

        public void DrawTool()
        {
            switch (tool)
            {
                case Tool.JSON:
                    DrawMethod(_JSONPersistance);
                    break;
                case Tool.XML:
                    DrawMethod(_XMLPersistance);
                    break;
                case Tool.Binary:
                    DrawMethod(_BinaryPersistance);
                    break;
                default:
                    break;
            }
        }

        SerializedProperty GetProperty(int index)
        {
            switch (tool)
            {
                case Tool.JSON:
                    switch (index)
                    {
                        case 0:
                            return j_UseApplicationPath;
                        case 1:
                            return j_SaveName;
                        case 2:
                            return j_ExtensionName;
                        case 3:
                            return j_CustomPath;
                        default:
                            break;
                    }
                    break;
                case Tool.XML:
                    switch (index)
                    {
                        case 0:
                            return x_UseApplicationPath;
                        case 1:
                            return x_SaveName;
                        case 2:
                            return x_ExtensionName;
                        case 3:
                            return x_CustomPath;
                        default:
                            break;
                    }
                    break;
                case Tool.Binary:
                    switch (index)
                    {
                        case 0:
                            return b_UseApplicationPath;
                        case 1:
                            return b_SaveName;
                        case 2:
                            return b_ExtensionName;
                        case 3:
                            return b_CustomPath;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        bool IsCustom()
        {
            switch (tool)
            {
                case Tool.JSON:
                    return TPSaveLoadDesigner.SaveLoadCreator.JSONMethod.UseApplicationPath == TP_SaveLoad.TPSaveLoadCreator.ApplicationPath.Custom;
                case Tool.XML:
                    return TPSaveLoadDesigner.SaveLoadCreator.XMLMethod.UseApplicationPath == TP_SaveLoad.TPSaveLoadCreator.ApplicationPath.Custom;
                case Tool.Binary:
                    return TPSaveLoadDesigner.SaveLoadCreator.BinaryMethod.UseApplicationPath == TP_SaveLoad.TPSaveLoadCreator.ApplicationPath.Custom;
                default:
                    break;
            }
            return false;
        }

        void DrawMethod(SerializedProperty property)
        {
            property.serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.BeginVertical();

            Space(3);
            EditorGUILayout.LabelField(content0, TPSaveLoadDesigner.skin.GetStyle("TipLabel"));
            EditorGUILayout.PropertyField(GetProperty(0), GUIContent.none);

            Space(3);
            EditorGUILayout.LabelField(content1, TPSaveLoadDesigner.skin.GetStyle("TipLabel"));
            EditorGUILayout.PropertyField(GetProperty(1), GUIContent.none);

            Space(3);
            EditorGUILayout.LabelField(content2, TPSaveLoadDesigner.skin.GetStyle("TipLabel"));
            EditorGUILayout.PropertyField(GetProperty(2), GUIContent.none);

            if (IsCustom())
            {
                Space(3);
                EditorGUILayout.LabelField(content3, TPSaveLoadDesigner.skin.GetStyle("TipLabel"));
                EditorGUILayout.PropertyField(GetProperty(3), GUIContent.none);
            }

            if (GUI.changed)
                property.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
        }
        
        void Space(int length)
        {
            for (int i = 0; i < length; i++)
                EditorGUILayout.Space();
        }

        void Update()
        {
            if (EditorApplication.isCompiling)
                this.Close();
        }
    }
}