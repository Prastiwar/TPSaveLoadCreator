using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objs
{
    public string Key;
    public object Value;

    public Objs(string _Key, UnityEngine.Object _Value)
    {
        Key = _Key;
        Value = _Value;
    }
}

[CreateAssetMenu()]
public class TPSaveLoadData : ScriptableObject
{
    public List<Objs> PersistanceObjects = new List<Objs>();

    public bool ContainsKey(string _Key)
    {
        int length = PersistanceObjects.Count;

        for (int i = 0; i < length; i++)
        {
            if (PersistanceObjects[i].Key == _Key)
            {
                return true;
            }
        }
        return false;
    }
}
