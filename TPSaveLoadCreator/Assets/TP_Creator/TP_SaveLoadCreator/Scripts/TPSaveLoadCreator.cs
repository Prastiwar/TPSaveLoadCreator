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

        List<object> PersistanceObjects = new List<object>();
        public List<int> PersistanceObjectsID = new List<int>();

        void Start()
        {
            Load();
        }
        void OnApplicationQuit()
        {
            Save();
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
            PersistanceObjectsID.Clear();
            PersistanceAttribute(true);
            SaveID();
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
            List<System.Object> objects = serializedObject as List<System.Object>;
            PersistanceObjects = objects;
            LoadID();
            PersistanceAttribute(false);
            file.Close();
        }
        void SaveID()
        {
            string path = GetSaveLoadPath() + "ID";
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileID = File.Create(path);
            bf.Serialize(fileID, PersistanceObjectsID);
        }
        void LoadID()
        {
            string path = GetSaveLoadPath() + "ID";
            if (!File.Exists(path))
                return;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileID = File.Open(path, FileMode.Open);
            object serializedObjectID = bf.Deserialize(fileID);
            List<int> objectsID = serializedObjectID as List<int>;
            PersistanceObjectsID = objectsID;
            fileID.Close();
        }

        void PersistanceAttribute(bool ToSave)
        {
            MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();
            int realIndex = 0;
            foreach (MonoBehaviour mono in sceneActive)
            {
                FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                if(ToSave)
                    PersistanceObjectsID.Add(mono.GetInstanceID());

                for (int i = 0; i < objectFields.Length; i++)
                {
                    Persistance attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(Persistance)) as Persistance;
                    if (attribute != null)
                    {
                        if (ToSave)
                        {
                            PersistanceObjects.Add(objectFields[i].GetValue(mono));
                        }
                        else
                        {
                            foreach (int IDIndex in PersistanceObjectsID)
                            {
                                Debug.Log(IDIndex);
                                if (mono.GetInstanceID() == PersistanceObjectsID[IDIndex])
                                {
                                    objectFields[i].SetValue(mono, PersistanceObjects[realIndex]);
                                }
                            }
                            realIndex++;
                        }
                    }

                }
            }
        }

    }

    [AttributeUsage(AttributeTargets.Field)]
    public class Persistance : Attribute { }
}