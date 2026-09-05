using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PoolData
{
    public int id; //ID
    public string Name; //名前(インスペクタ上で区別用)
    public PoolableObject prefab;//ゲームオブジェクト

}

[System.Serializable]
[CreateAssetMenu(fileName = "PoolDataDB", menuName = "Scriptable Objects/PoolDataDB")]
public class PoolDataDB : ScriptableObject
{
    public List<PoolData> data = new List<PoolData>(); //リスト
}
