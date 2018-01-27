using System;
using System.Collections;
using System.Collections.Generic;
using TP_SaveLoad;
using UnityEngine;

[ExecuteInEditMode]
public class TPSaveLoadPersistance : MonoBehaviour
{
    public string ID;
    TPSaveLoadCreator creator;
    public MonoBehaviour[] monos;

    void OnValidate()
    {
        //hideFlags = HideFlags.HideInInspector;
        //hideFlags = HideFlags.None;
        UnityEditor.MonoScript myScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
        if (UnityEditor.MonoImporter.GetExecutionOrder(myScript) < 80)
            UnityEditor.MonoImporter.SetExecutionOrder(myScript, 80);

        monos = GetComponents<MonoBehaviour>();
    }

    void Awake()
    {
        creator = FindObjectOfType<TPSaveLoadCreator>();
        Persistance(false);
    }

    void Persistance(bool ToSave)
    {
        int length = monos.Length;
        for (int i = 0; i < length; i++)
        {
            if(ToSave)
                creator.SaveObject(monos[i], ID);
            else
                creator.LoadObject(monos[i], ID);
        }
    }

    void OnApplicationQuit()
    {
        Persistance(true);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            Persistance(true);
    }
}