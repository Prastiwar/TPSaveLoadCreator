using UnityEditor;
using TP.SaveLoad;

namespace TP.SaveLoadEditor
{
    [CustomEditor(typeof(TPSaveLoadCreator))]
    public class TPSaveLoadCreatorEditor : ScriptlessSaveLoadEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Script which allows you managing persistance");
            if (TPSaveLoadCreator.DebugMode)
                DrawPropertiesExcluding(serializedObject, scriptField);

            OpenCreator();
        }

    }
}