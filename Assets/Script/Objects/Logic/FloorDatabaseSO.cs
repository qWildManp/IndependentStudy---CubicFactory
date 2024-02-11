using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FloorDatabaseSO : ScriptableObject
{
    public List<FloorData> floors;
}

// [Serializable]
[CreateAssetMenu(fileName = "FloorData", menuName = "Data/FloorData")]
public class FloorData : ScriptableObject
{
    [field: SerializeField]
    public string flrName { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector2Int size { get; private set; } = Vector2Int.one;
    // [field: SerializeField]
    // public GameObject prefab { get; private set; }
    [field: SerializeField]
    public bool isAccessible { get; private set; } = true;

}