using UnityEngine;
using UnityEditor;

namespace TP_SaveLoadEditor
{
    [CustomEditor(typeof(TPSaveLoadGUIData))]
    public class TPSoundManagerGUIDataEditor : ScriptlessSaveLoadEditor
    {
        TPSaveLoadGUIData TPSoundData;

        void OnEnable()
        {
            TPSoundData = (TPSaveLoadGUIData)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("GUI Skin");
            TPSoundData.GUISkin =
                (EditorGUILayout.ObjectField(TPSoundData.GUISkin, typeof(GUISkin), true) as GUISkin);

            if (GUI.changed)
                EditorUtility.SetDirty(TPSoundData);

            serializedObject.ApplyModifiedProperties();
        }
    }
}