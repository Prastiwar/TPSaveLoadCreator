using System.Collections;
using System.Collections.Generic;
using TP_SaveLoad;
using UnityEngine;

[System.Serializable]
public class DemoSaveLoadScript2 : MonoBehaviour
{
    [Persistance]
    public string _String2;

    [Persistance]
    public bool _Bool2;

    [Persistance]
    public int _Int2;

    [Persistance]
    public float _Float2;
}
