using UnityEngine;
using UnityEditor;

namespace TP_SaveLoadEditor
{
    public class ScriptlessSaveLoadEditor : Editor
    {
        public readonly string scriptField = "m_Script";

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, scriptField);

            OpenCreator();
        }

        public void OpenCreator()
        {
            if (GUILayout.Button("Open Sound Manager", GUILayout.Height(30)))
            {
                TPSaveLoadDesigner.OpenWindow();
            }
        }
    }
}