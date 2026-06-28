using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShotData
{
    public int id;
    public string name;
    public float moveSpeed;
    public float lifeSpan;
    public float rad;
    public int hitEffectID;
}
[System.Serializable]
public class ShotDataList
{
    public List<ShotData> shotDatas;
}
public class ShotDataManager
{
    private Dictionary<int, ShotData> shotDataDict = new Dictionary<int, ShotData>();
    
    //JSONから弾データをロード
    public void LoadShotDB()
    {
        if(shotDataDict.Count > 0) return;

        TextAsset jsonText = Resources.Load<TextAsset>("Data/ShotDB/ShotDatas");

        if(jsonText != null){
            ShotDataList list = JsonUtility.FromJson<ShotDataList>(jsonText.text);

            foreach(ShotData data in list.shotDatas){
                shotDataDict.Add(data.id, data);
            }
        }
        else{
            Debug.LogError("ShotDB.jsonがありませんでした。");
        }
    }
    //IDに相当する弾データ返却
    public ShotData GetShotDataByID(int id)
    {
         if(shotDataDict.TryGetValue(id, out ShotData data))
        {
            return data;
        }
        Debug.LogError($"{id}に相当するshotデータはありません。");
        return null;
    }
}
