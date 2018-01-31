using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP_SaveLoad;

public class DemoSaveLoadScript : MonoBehaviour
{
    [PersistantBinary]
    public string _String;

    //[PersistantJSON("This is BOOL")]
    //public bool _Bool;

    //[PersistantJSON("This is INT")]
    //public int _Int;

    //[Persistance]
    //public float _Float;

    //[Persistance]
    //public int[] _Array;

    //[Persistance]
    //public Vector2 _Vector2;

    //[Persistance]
    //public UnityEngine.Object _UnityObject;

    //[PersistantXML("ObjTest")]
    //public GameObject _GameObject;

    //[Persistance]
    //public DemoSaveLoadScript2 _DemoScript2;
}