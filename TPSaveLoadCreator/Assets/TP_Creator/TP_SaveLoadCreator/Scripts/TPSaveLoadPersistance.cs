using System;
using System.Collections;
using System.Collections.Generic;
using TP_SaveLoad;
using UnityEngine;

public class TPSaveLoadPersistance : MonoBehaviour
{
    public string ID;
    TPSaveLoadCreator creator;

    void OnValidate()
    {
        UnityEditor.MonoScript myScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
        if (UnityEditor.MonoImporter.GetExecutionOrder(myScript) < 80)
            UnityEditor.MonoImporter.SetExecutionOrder(myScript, 80);
    }

    void Awake()
    {
        creator = FindObjectOfType<TPSaveLoadCreator>();
        creator.LoadObject(this, ID);
    }

    void OnApplicationQuit()
    {
        creator.SaveObject(this, ID);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            creator.SaveObject(this, ID);
    }
}