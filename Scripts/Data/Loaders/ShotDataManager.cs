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

        if(jsonText == null)
        {
            throw new System.IO.FileNotFoundException(
                "弾データが見つかりませんでした:Resources/Data/ShotDB/ShotDatas"
            );
        }

        ShotDataList list = JsonUtility.FromJson<ShotDataList>(jsonText.text);
        if(list?.shotDatas == null || list.shotDatas.Count == 0)
        {
            throw new System.Exception("弾データのパースに失敗、または空きです。：ShotDB/ShotDatas");
        }

        foreach(ShotData data in list.shotDatas){
            shotDataDict.Add(data.id, data);
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
