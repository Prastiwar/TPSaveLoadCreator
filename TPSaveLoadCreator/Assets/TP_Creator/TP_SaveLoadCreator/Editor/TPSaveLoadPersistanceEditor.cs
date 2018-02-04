using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TP.SaveLoad;

[CustomEditor(typeof(TPSaveLoadPersistance))]
public class TPSaveLoadPersistanceEditor : Editor
{
    List<TPSaveLoadPersistance> list = new List<TPSaveLoadPersistance>();
    TPSaveLoadPersistance thisObject;
    int length;

    void OnEnable()
    {
        thisObject = target as TPSaveLoadPersistance;
        list = FindListObjectsOfType<TPSaveLoadPersistance>();
        length = list.Count;
    }

    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, "m_Script");

        for (int i = 0; i < length; i++)
        {
            if (thisObject.name != list[i].name) 
            {
                if (thisObject.ID == list[i].ID)
                {
                    thisObject.ID = System.Guid.NewGuid().ToString();//"PersistantObject_" + i;
                    Repaint();
                    break;
                }
            }
        }
    }

    List<T> FindListObjectsOfType<T>() where T : UnityEngine.Object
    {
        T[] objArray = FindObjectsOfType<T>();
        int length = objArray.Length;
        List<T> objList = new List<T>(length);

        for (int i = 0; i < length; i++)
        {
            objList.Add(objArray[i]);
        }

        return objList;
    }
}
