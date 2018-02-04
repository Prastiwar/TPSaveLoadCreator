using UnityEngine;
using UnityEditor;
using TP.SaveLoad;

namespace TP.SaveLoadEditor
{
    [CustomEditor(typeof(TPSaveLoadGUIData))]
    public class TPSoundManagerGUIDataEditor : ScriptlessSaveLoadEditor
    {
        TPSaveLoadGUIData TPSaveLoadData;

        void OnEnable()
        {
            TPSaveLoadData = (TPSaveLoadGUIData)target;
            if (serializedObject.targetObject.hideFlags != HideFlags.NotEditable)
                serializedObject.targetObject.hideFlags = HideFlags.NotEditable;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Container for editor data");
            if (!TPSaveLoadCreator.DebugMode)
                return;

            EditorGUILayout.LabelField("GUI Skin");
            TPSaveLoadData.GUISkin =
                (EditorGUILayout.ObjectField(TPSaveLoadData.GUISkin, typeof(GUISkin), true) as GUISkin);

            if (GUI.changed)
                EditorUtility.SetDirty(TPSaveLoadData);
        }
    }
}