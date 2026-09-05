using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public int id;
    public string name;
    public int currentHP;
    public int maxHP;
    public int attack;
    public int defense;
    public float moveSpeed;
    public float attackDist;
    public float attackSpeed;
    public float rad;
    public float rewardEXP;

    public EnemyData Clone()
    {
        EnemyData clone = new EnemyData();

        clone.id = this.id;
        clone.name = this.name;
        clone.maxHP = this.maxHP;
        clone.attack = this.attack;
        clone.defense = this.defense;
        clone.moveSpeed = this.moveSpeed;
        clone.attackDist = this.attackDist;
        clone.attackSpeed = this.attackSpeed;
        clone.rad = this.rad;
        clone.rewardEXP = this.rewardEXP;

        return clone;
    }
}
[System.Serializable]
public class EnemyDataList
{
    public List<EnemyData> enemyDatas;
}
public class EnemyDataManager
{
    private Dictionary<int, EnemyData> enemyDataDict = new Dictionary<int, EnemyData>();
    
    public void LoadEnemyDB()
    {
        if(enemyDataDict.Count > 0) return;

        TextAsset jsonText = Resources.Load<TextAsset>("Data/EnemyDB/EnemyDatas");

        if(jsonText == null)
        {
            throw new System.IO.FileNotFoundException(
                "敵データが見つかりません:Resources/Data/EnemyDB/EnemyDatas"
            );
        }

        
        EnemyDataList list = JsonUtility.FromJson<EnemyDataList>(jsonText.text);
            
        if(list?.enemyDatas == null || list.enemyDatas.Count == 0)
        {
            throw new System.Exception("敵データのパースに失敗、または空きです。：EnemyDB/EnemyDatas");
        }
        foreach(EnemyData data in list.enemyDatas){
            enemyDataDict.Add(data.id, data);
        }

    }

    public EnemyData GetEnemyDataByID(int id)
    {
         if(enemyDataDict.TryGetValue(id, out EnemyData data))
        {
            return data.Clone();
        }
        Debug.LogError($"{id}に相当するEnemyデータはありません。");
        return null;
    }

    
}
