using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PoolData
{
    public int id;
    public string Name;
    public PoolableObject prefab;

}

[System.Serializable]
[CreateAssetMenu(fileName = "PoolDataDB", menuName = "Scriptable Objects/PoolDataDB")]
public class PoolDataDB : ScriptableObject
{
    public List<PoolData> data = new List<PoolData>();
}
