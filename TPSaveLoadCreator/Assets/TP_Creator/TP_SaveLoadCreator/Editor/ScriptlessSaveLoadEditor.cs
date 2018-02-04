using UnityEngine;
using UnityEditor;
using TP.SaveLoad;

namespace TP.SaveLoadEditor
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
            if (TPSaveLoadCreator.DebugMode)
            {
                if (serializedObject.targetObject.hideFlags != HideFlags.NotEditable)
                    serializedObject.targetObject.hideFlags = HideFlags.NotEditable;
                return;
            }

            if (serializedObject.targetObject.hideFlags != HideFlags.None)
                serializedObject.targetObject.hideFlags = HideFlags.None;

            if (GUILayout.Button("Open SaveLoad Manager", GUILayout.Height(30)))
                TPSaveLoadDesigner.OpenWindow();
        }
    }
}