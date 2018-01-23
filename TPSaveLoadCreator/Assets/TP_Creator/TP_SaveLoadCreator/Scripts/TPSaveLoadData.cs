using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TPSaveLoadData : ScriptableObject
{
    public Dictionary<string, object> PersistanceObjects = new Dictionary<string, object>();
}
