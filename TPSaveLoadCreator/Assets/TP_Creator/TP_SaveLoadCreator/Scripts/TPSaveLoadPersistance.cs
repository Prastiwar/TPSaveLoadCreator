﻿using System;
using System.Collections;
using System.Collections.Generic;
using TP.SaveLoad;
using UnityEngine;

public class TPSaveLoadPersistance : MonoBehaviour
{
    [PersistantID]
    public string ID;
    TPSaveLoadCreator creator;
    public MonoBehaviour[] monos;

    void OnValidate()
    {
        UnityEditor.MonoScript myScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
        if (UnityEditor.MonoImporter.GetExecutionOrder(myScript) < 80)
            UnityEditor.MonoImporter.SetExecutionOrder(myScript, 80);
        if (ID == null) ID = Guid.NewGuid().ToString();
        
        monos = GetComponents<MonoBehaviour>();
    }
   
    public void Refresh()
    {
        OnValidate();
        if (TPSaveLoadCreator.DebugMode)
        {
            if (hideFlags != HideFlags.NotEditable)
                hideFlags = HideFlags.NotEditable;
        }
        else
        {
            if (hideFlags != HideFlags.HideInInspector)
                hideFlags = HideFlags.HideInInspector;
        }
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