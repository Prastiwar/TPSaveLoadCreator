using UnityEngine;
using UnityEditor;
using TP_SaveLoad;

namespace TP_SaveLoadEditor
{
    [CustomEditor(typeof(TPSaveLoadGUIData))]
    public class TPSoundManagerGUIDataEditor : ScriptlessSaveLoadEditor
    {
        TPSaveLoadGUIData TPSoundData;

        void OnEnable()
        {
            TPSoundData = (TPSaveLoadGUIData)target;
            if (serializedObject.targetObject.hideFlags != HideFlags.NotEditable)
                serializedObject.targetObject.hideFlags = HideFlags.NotEditable;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Container for editor data");
            if (!TPSaveLoadCreator.DebugMode)
                return;

            EditorGUILayout.LabelField("GUI Skin");
            TPSoundData.GUISkin =
                (EditorGUILayout.ObjectField(TPSoundData.GUISkin, typeof(GUISkin), true) as GUISkin);

            if (GUI.changed)
                EditorUtility.SetDirty(TPSoundData);
        }
    }
}