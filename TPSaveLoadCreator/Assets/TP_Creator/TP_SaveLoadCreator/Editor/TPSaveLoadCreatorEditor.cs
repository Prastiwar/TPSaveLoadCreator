using UnityEditor;
using TP_SaveLoad;

namespace TP_SaveLoadEditor
{
    //[CustomEditor(typeof(TPSaveLoadCreator))]
    public class TPSaveLoadCreatorEditor : ScriptlessSaveLoadEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Script which allows you managing persistance");
            OpenCreator();
        }

    }
}