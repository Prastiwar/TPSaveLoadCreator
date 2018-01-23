using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TP_SaveLoad
{
    public class TPSaveLoadCreator : MonoBehaviour
    {
        public enum ApplicationPath
        {
            PersistentDataPath,
            DataPath,
            StreamingAssetsPath,
            temporaryCachePath
        }

        public bool UseCustomPath;
        public string CustomPath;

        public ApplicationPath UseApplicationPath;
        public string SaveName;
        public string ExtensionName;

        public Dictionary<string, object> PersistanceObjects = new Dictionary<string, object>();
        //public TPSaveLoadData PersistanceData;

        void OnValidate()
        {
            UnityEditor.MonoScript myScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
            if(UnityEditor.MonoImporter.GetExecutionOrder(myScript) < 50)
                UnityEditor.MonoImporter.SetExecutionOrder(myScript, 50);
        }
        void Awake()
        {
            Debug.Log("Awake");
            Load();
        }
        void OnDestroy()
        {
            Debug.Log("Destroy");
            Save();
        }

        public void SaveObject(MonoBehaviour mono, string ID)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int fieldsLength = objectFields.Length;
            for (int i = 0; i < fieldsLength; i++)
            {
                Persistance attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(Persistance)) as Persistance;
                if (attribute != null)
                {
                    string TKey = ID + i;

                    if (!PersistanceObjects.ContainsKey(TKey))
                    {
                        PersistanceObjects.Add(TKey, objectFields[i].GetValue(mono));
                    }
                    else
                    {
                        PersistanceObjects.Remove(TKey);
                        PersistanceObjects.Add(TKey, objectFields[i].GetValue(mono));
                    }
                }
            }
            Debug.Log("Save done " + mono);
        }

        public void LoadObject(MonoBehaviour mono, string ID)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int fieldsLength = objectFields.Length;
            for (int i = 0; i < fieldsLength; i++)
            {
                Persistance attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(Persistance)) as Persistance;
                if (attribute != null)
                {
                    string TKey = ID + i;

                    if (PersistanceObjects.ContainsKey(TKey))
                    {
                        objectFields[i].SetValue(mono, PersistanceObjects[TKey]);
                    }
                }
            }
            Debug.Log("Load Done " + mono);
        }
    
        public string GetSaveLoadPath()
        {
            if (UseCustomPath)
            {
                return CustomPath;
            }
            else
            {
                switch (UseApplicationPath)
                {
                    case ApplicationPath.PersistentDataPath:
                        return Application.persistentDataPath + "/" + SaveName + "." + ExtensionName;
                    case ApplicationPath.DataPath:
                        return Application.dataPath + "/" + SaveName + "." + ExtensionName;
                    case ApplicationPath.StreamingAssetsPath:
                        return Application.streamingAssetsPath + "/" + SaveName + "." + ExtensionName;
                    case ApplicationPath.temporaryCachePath:
                        return Application.temporaryCachePath + "/" + SaveName + "." + ExtensionName;
                    default:
                        break;
                }
            }
            return null;
        }

        public void Save()
        {
            string path = GetSaveLoadPath();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            //if (PersistanceObjects != null)
            //    PersistanceObjects.Clear();
            //PersistanceAttribute(true);
            bf.Serialize(file, PersistanceObjects);
        }

        public void Load()
        {
            string path = GetSaveLoadPath();
            if (!File.Exists(path))
                return;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            object serializedObject = bf.Deserialize(file);
            Dictionary<string, object> objects = serializedObject as Dictionary<string, object>;
            PersistanceObjects = objects;
            //PersistanceAttribute(false);
            file.Close();
        }

        //void PersistanceAttribute(bool ToSave)
        //{
        //    if (ToSave)
        //        PersistanceObjects = new Dictionary<string, object>();

        //    MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
        //    int length = sceneActive.Length;

        //    for (int m = 0; m < length; m++)
        //    {
        //        var mono = sceneActive[m];
        //        FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

        //        int fieldsLength = objectFields.Length;
        //        for (int i = 0; i < fieldsLength; i++)
        //        {
        //            Persistance attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(Persistance)) as Persistance;
        //            if (attribute != null)
        //            {
        //                TPSaveLoadPersistance persistance = mono.GetComponent<TPSaveLoadPersistance>();
        //                if (persistance == null)
        //                    Debug.Log("error");

        //                string TKey = persistance.ID.ToString() + i;

        //                if (ToSave)
        //                {
        //                    if (!PersistanceObjects.ContainsKey(TKey))
        //                    {
        //                        PersistanceObjects.Add(TKey, objectFields[i].GetValue(mono));
        //                    }
        //                }
        //                else
        //                {
        //                    if (PersistanceObjects.ContainsKey(TKey))
        //                    {
        //                        objectFields[i].SetValue(mono, PersistanceObjects[TKey]);
        //                    }
        //                }

        //            }
        //        }
        //    }

        //}
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Persistance : Attribute { }
}