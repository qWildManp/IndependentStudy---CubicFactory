using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objects;
}


// [Serializable]
[CreateAssetMenu(fileName = "ObjectData", menuName = "Data/ObjectData")]
public class ObjectData : ScriptableObject
{
    [field: SerializeField]
    public string objName { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector3Int size { get; private set; } = Vector3Int.one;
    // [field: SerializeField]
    // public GameObject prefab { get; private set; }

}