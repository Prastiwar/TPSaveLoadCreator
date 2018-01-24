using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class Objs
{
    public string Key;
    public UnityEngine.Object Value;

    public Objs(string _Key, UnityEngine.Object _Value)
    {
        Key = _Key;
        Value = _Value;
    }

    //public void GetObjectData(SerializationInfo info, StreamingContext context)
    //{
    //    info.AddValue("Value", Value.GetType());
    //}
    //public Objs(SerializationInfo info, StreamingContext context)
    //{
    //    Value = info.GetValue("Value", typeof(UnityEngine.Object)) as UnityEngine.Object;
    //    if (Value == null)
    //        return;
    //}

    //public Objs()
    //{
    //}
}

[CreateAssetMenu()]
[System.Serializable]
public class TPSaveLoadData : ScriptableObject//, ISerializable
{
    [SerializeField]
    public List<Objs> PersistanceObjects = new List<Objs>();
    [SerializeField]
    public Hashtable data = new Hashtable();

    public void aha()
    {
        data.Add("0", PersistanceObjects[0]);
    }
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

    //public TPSaveLoadData()
    //{
    //    // Empty constructor required to compile.
    //}

    //// Implement this method to serialize data. The method is called on serialization.
    //public void GetObjectData(SerializationInfo info, StreamingContext context)
    //{
    //    info.AddValue("Value", PersistanceObjects.GetType());
    //}

    //// The special constructor is used to deserialize values.
    //// In this case, it recreate the original ScriptableObject.
    //public TPSaveLoadData(SerializationInfo info, StreamingContext context)
    //{
    //    PersistanceObjects = info.GetValue("Value", typeof(List<Objs>)) as List<Objs>;
    //    if (PersistanceObjects == null)
    //        return;
    //}
}
