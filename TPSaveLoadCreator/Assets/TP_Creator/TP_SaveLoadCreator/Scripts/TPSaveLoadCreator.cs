using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace TP.SaveLoad
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class PersistantBinary : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class PersistantJSON : PropertyAttribute
    {
        public string ElementName;
        public PersistantJSON(string _ElementName)
        {
            ElementName = _ElementName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class PersistantXML : PropertyAttribute
    {
        public string ElementName;
        public PersistantXML(string _ElementName)
        {
            ElementName = _ElementName;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PersistantID : Attribute { }

    public class TPSaveLoadCreator : MonoBehaviour
    {
        public static bool DebugMode;

        [System.Serializable]
        public class ReadableHolder
        {
            public string Key;
            internal object Value;
            public string _Value;
            public ReadableHolder(string key, object value)
            {
                Key = key;
                Value = value;
                if (Value != null)
                    _Value = Value.ToString();
                else
                    _Value = null;
            }
            private ReadableHolder()
            { }
        }
        [System.Serializable]
        public class ReadableWrapper
        {
            public List<ReadableHolder> ReadablePersistance = new List<ReadableHolder>();
            
            public ReadableWrapper()
            { }
        }

        [System.Serializable]
        public struct Method
        {
            public ApplicationPath UseApplicationPath;
            public string SaveName;
            public string ExtensionName;
            public string CustomPath;
        }

        public enum ApplicationPath
        {
            Custom,
            PersistentDataPath,
            DataPath,
            StreamingAssetsPath,
            temporaryCachePath
        }

        public enum MethodEnum
        {
            BinaryMethod,
            JSONMethod,
            XMLMethod
        }
        public Method BinaryMethod;
        public Method JSONMethod;
        public Method XMLMethod;
        Method IDMethod;

        [SerializeField, HideInInspector] Dictionary<string, object> PersistanceObjects = new Dictionary<string, object>();
        [SerializeField, HideInInspector] Dictionary<string, object> PersistanceIDs = new Dictionary<string, object>();
        [SerializeField, HideInInspector] ReadableWrapper JSONWrapper = new ReadableWrapper();
        [SerializeField, HideInInspector] ReadableWrapper XMLWrapper = new ReadableWrapper();

        void OnValidate()
        {
            UnityEditor.MonoScript myScript = UnityEditor.MonoScript.FromMonoBehaviour(this);
            if (UnityEditor.MonoImporter.GetExecutionOrder(myScript) < 50)
                UnityEditor.MonoImporter.SetExecutionOrder(myScript, 50);

            CheckMonosPersistent();
        }
        void Awake()
        {
            SetIDMethod();
            LoadID();
            LoadBIN();
            LoadJSON();
            LoadXML();
        }
        void OnDestroy()
        {
            SaveID();
            SaveBIN();
            SaveJSON();
            SaveXML();
        }

        void CheckMonosPersistent()
        {
            MonoBehaviour[] monos = FindObjectsOfType<MonoBehaviour>();
            foreach (var mono in monos)
            {
                FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                int fieldsLength = objectFields.Length;
                for (int i = 0; i < fieldsLength; i++)
                {
                    PersistantBinary attBinary = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantBinary)) as PersistantBinary;
                    PersistantJSON attJSON = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantJSON)) as PersistantJSON;
                    PersistantXML attXML = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantXML)) as PersistantXML;
                    PersistantID attID = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantID)) as PersistantID;

                    if (attBinary != null || attJSON != null || attXML != null || attID != null)
                    {
                        var persist = mono.GetComponent<TPSaveLoadPersistance>();
                        if (persist == null)
                            mono.gameObject.AddComponent<TPSaveLoadPersistance>();
                    }
                }
            }
        }

        public void SaveObject(MonoBehaviour mono, string ID)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int fieldsLength = objectFields.Length;
            for (int i = 0; i < fieldsLength; i++)
            {
                PersistantBinary attBinary = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantBinary)) as PersistantBinary;
                PersistantJSON attJSON = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantJSON)) as PersistantJSON;
                PersistantXML attXML = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantXML)) as PersistantXML;
                PersistantID attID = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantID)) as PersistantID;

                string TKey = ID + mono.GetType() + i;

                if (attBinary != null)
                {
                    if (!PersistanceObjects.ContainsKey(TKey))
                    {
                        PersistanceObjects.Add(TKey, objectFields[i].GetValue(mono));
                    }
                    else
                    {
                        PersistanceObjects[TKey] = objectFields[i].GetValue(mono);
                    }
                }

                if (attJSON != null)
                {
                    ReadableHolder holder = new ReadableHolder(attJSON.ElementName, objectFields[i].GetValue(mono));
                    int index = ReadableContainsKey(JSONWrapper, attJSON.ElementName);
                    if (index > -1)
                    {
                        JSONWrapper.ReadablePersistance[index] = holder;
                    }
                    else
                    {
                        JSONWrapper.ReadablePersistance.Add(holder);
                    }
                }

                if (attID != null)
                {
                    if (!PersistanceIDs.ContainsKey(TKey))
                    {
                        PersistanceIDs.Add(TKey, objectFields[i].GetValue(mono));
                    }
                    else
                    {
                        PersistanceIDs[TKey] = objectFields[i].GetValue(mono);
                    }
                }

                if (attXML != null)
                {
                    ReadableHolder holder = new ReadableHolder(attXML.ElementName, objectFields[i].GetValue(mono));
                    int index = ReadableContainsKey(XMLWrapper, attXML.ElementName);
                    if (index > -1)
                    {
                        XMLWrapper.ReadablePersistance[index] = holder;
                    }
                    else
                    {
                        XMLWrapper.ReadablePersistance.Add(holder);
                    }
                }
            }
        }

        public void LoadObject(MonoBehaviour mono, string ID)
        {
            FieldInfo[] objectFields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int fieldsLength = objectFields.Length;
            for (int i = 0; i < fieldsLength; i++)
            {
                PersistantBinary attBinary = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantBinary)) as PersistantBinary;
                PersistantJSON attJSON = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantJSON)) as PersistantJSON;
                PersistantXML attXML = PropertyAttribute.GetCustomAttribute(objectFields[i], typeof(PersistantXML)) as PersistantXML;
                PersistantID attID = Attribute.GetCustomAttribute(objectFields[i], typeof(PersistantID)) as PersistantID;

                string TKey = ID + mono.GetType() + i;

                try
                {
                    if (attBinary != null)
                    {
                        if (PersistanceObjects.ContainsKey(TKey))
                            objectFields[i].SetValue(mono, PersistanceObjects[TKey]);
                    }
                    if (attJSON != null)
                    {
                        int index = ReadableContainsKey(JSONWrapper, attJSON.ElementName);
                        if (index > -1)
                        {
                            Type type = objectFields[i].FieldType;
                            object convertedValue = Convert.ChangeType(JSONWrapper.ReadablePersistance[index]._Value, type);
                            objectFields[i].SetValue(mono, convertedValue);
                        }
                    }
                    if (attID != null)
                    {
                        if (PersistanceIDs.ContainsKey(TKey))
                            objectFields[i].SetValue(mono, PersistanceIDs[TKey]);
                    }
                    if (attXML != null)
                    {
                        int index = ReadableContainsKey(XMLWrapper, attXML.ElementName);
                        if (index > -1)
                        {
                            Type type = objectFields[i].FieldType;
                            object convertedValue = Convert.ChangeType(XMLWrapper.ReadablePersistance[index]._Value, type);
                            objectFields[i].SetValue(mono, convertedValue);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Loading error");
                    throw ex;
                }
            }
        }

        public string GetSaveLoadPath(MethodEnum methodEnum)
        {
            Method method = new Method();

            switch (methodEnum)
            {
                case MethodEnum.BinaryMethod:
                    method = BinaryMethod;
                    break;
                case MethodEnum.JSONMethod:
                    JSONMethod.ExtensionName = "json";
                    method = JSONMethod;
                    break;
                case MethodEnum.XMLMethod:
                    JSONMethod.ExtensionName = "xml";
                    method = XMLMethod;
                    break;
                default:
                    break;
            }

            string _SaveName = Path.ChangeExtension(method.SaveName, method.ExtensionName);

            switch (method.UseApplicationPath)
            {
                case ApplicationPath.Custom:
                    return method.CustomPath;
                case ApplicationPath.PersistentDataPath:
                    return Path.Combine(Application.persistentDataPath, _SaveName);
                case ApplicationPath.DataPath:
                    return Path.Combine(Application.dataPath, _SaveName);
                case ApplicationPath.StreamingAssetsPath:
                    return Path.Combine(Application.streamingAssetsPath, _SaveName);
                case ApplicationPath.temporaryCachePath:
                    return Path.Combine(Application.temporaryCachePath, _SaveName);
                default:
                    break;
            }
            return null;
        }

        string GetIDPath()
        {
            Method method = new Method();
            method = IDMethod;

            string _SaveName = Path.ChangeExtension(method.SaveName, method.ExtensionName);

            switch (method.UseApplicationPath)
            {
                case ApplicationPath.Custom:
                    return method.CustomPath;
                case ApplicationPath.PersistentDataPath:
                    return Path.Combine(Application.persistentDataPath, _SaveName);
                case ApplicationPath.DataPath:
                    return Path.Combine(Application.dataPath, _SaveName);
                case ApplicationPath.StreamingAssetsPath:
                    return Path.Combine(Application.streamingAssetsPath, _SaveName);
                case ApplicationPath.temporaryCachePath:
                    return Path.Combine(Application.temporaryCachePath, _SaveName);
                default:
                    break;
            }
            return null;
        }

        public bool JSONContainsKey(string TKey)
        {
            int index = ReadableContainsKey(JSONWrapper, TKey);
            if (index == -1)
                return false;
            else
                return true;
        }

        public bool XMLContainsKey(string TKey)
        {
            int index = ReadableContainsKey(XMLWrapper, TKey);
            if (index == -1)
                return false;
            else
                return true;
        }

        public string GetObjectID(MonoBehaviour mono)
        {
            TPSaveLoadPersistance persistance = mono.GetComponent<TPSaveLoadPersistance>();

            if (persistance != null)
                return persistance.ID;

            Debug.Log("ID for '" + mono + "' not found");
            return null;
        }

        int ReadableContainsKey(ReadableWrapper Wrapper, string TKey)
        {
            int length = Wrapper.ReadablePersistance.Count;
            for (int i = 0; i < length; i++)
            {
                if (TKey == Wrapper.ReadablePersistance[i].Key)
                    return i;
            }
            return -1;
        }

        void SetIDMethod()
        {
            IDMethod.UseApplicationPath = ApplicationPath.PersistentDataPath;
            IDMethod.SaveName = "TP_IDs";
            IDMethod.ExtensionName = "dat";
        }

        void SaveBIN()
        {
            string path = GetSaveLoadPath(MethodEnum.BinaryMethod);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, PersistanceObjects);
            file.Close();
        }
        void LoadBIN()
        {
            string path = GetSaveLoadPath(MethodEnum.BinaryMethod);
            if (!File.Exists(path))
                return;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            object serializedObject = bf.Deserialize(file) as object;
            Dictionary<string, object> objects = serializedObject as Dictionary<string, object>;
            PersistanceObjects = objects;
            file.Close();
        }

        void SaveJSON()
        {
            string jsonToFile = JsonUtility.ToJson(JSONWrapper, true);
            string path = GetSaveLoadPath(MethodEnum.JSONMethod);
            File.WriteAllText(path, jsonToFile);
        }
        void LoadJSON()
        {
            string path = GetSaveLoadPath(MethodEnum.JSONMethod);
            if (!File.Exists(path))
                return;

            string jsonFromFile = File.ReadAllText(path);
            JSONWrapper = JsonUtility.FromJson<ReadableWrapper>(jsonFromFile);
        }

        void SaveXML()
        {
            string path = GetSaveLoadPath(MethodEnum.XMLMethod);
            var serializer = new XmlSerializer(typeof(ReadableWrapper));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, XMLWrapper);
            }
        }
        void LoadXML()
        {
            string path = GetSaveLoadPath(MethodEnum.XMLMethod);
            if (!File.Exists(path))
                return;

            var serializer = new XmlSerializer(typeof(ReadableWrapper));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                XMLWrapper = serializer.Deserialize(stream) as ReadableWrapper;
            }
        }

        void SaveID()
        {
            string path = GetIDPath();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            bf.Serialize(file, PersistanceIDs);
            file.Close();
        }
        void LoadID()
        {
            string path = GetIDPath();
            if (!File.Exists(path))
                return;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            object serializedObject = bf.Deserialize(file) as object;
            Dictionary<string, object> IDs = serializedObject as Dictionary<string, object>;
            PersistanceIDs = IDs;
            file.Close();
        }
    }
}