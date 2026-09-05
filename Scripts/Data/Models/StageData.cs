using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public enum StageType
{
    Normal, Boss, Bonus
}
//jsonファイル処理用宣言
[System.Serializable]
public class EnemySpawnData
{
    public int id; 
    public int x;
    public int z;
}

[System.Serializable]
public class ItemSpawnData
{
    public int id; 
    public int x;
    public int z;
}

[System.Serializable]
public class WaveData
{
    public int waveIndex;
    public List<EnemySpawnData> enemies;
    public List<ItemSpawnData> items;
}

[System.Serializable]
public class StageData
{
    public int stageIndex;
    public StageType stageType;
    public string stageMusic;
    public int RoomID;
    public List<WaveData> waveData;
}

[System.Serializable]
public class StageDataList
{
    public List<StageData> stages;
}
