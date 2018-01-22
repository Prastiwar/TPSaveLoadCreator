using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TP_SaveLoad;

public class DemoSaveLoadScript : MonoBehaviour
{
    [Persistance]
    public string _String;

    [Persistance]
    public bool _Bool;

    [Persistance]
    public int _Int;

    [Persistance]
    public float _Float;

    [Persistance]
    public int[] _Array;

    //void Awake()
    //{
    //    _Cameraper = _Camera;
    //}
    //void Start()
    //{
    //    Debug.Log(_Cameraper);
    //}

    //[Persistance]
    //public Object _Camera;

    //[Persistance]
    //public object _Cameraper;

    //[Persistance]
    //[SerializeField] public GameObject _GameObject;

    //[Persistance]
    //public DemoSaveLoadScript2 _DemoScript2;
}
