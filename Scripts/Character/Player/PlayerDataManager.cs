using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerDataList
{
    public List<PlayerStat> playerDatas;
}
//プレイヤのデータをファイルから読み込むためのクラス
public static class PlayerDataManager
{
    private static Dictionary<int, PlayerStat> playerDataDict = new Dictionary<int, PlayerStat>();

    public static int currentPlayerID;

    //DBのデータを読み込む
    public static void LoadPlayerDB()
    {
        if(playerDataDict.Count > 0) return;

        TextAsset jsonText = Resources.Load<TextAsset>("Data/PlayerDB/PlayerDatas");

        if(jsonText != null){
            PlayerDataList list = JsonUtility.FromJson<PlayerDataList>(jsonText.text);

            foreach(PlayerStat stat in list.playerDatas){
                playerDataDict.Add(stat.id, stat);
            }
            Debug.Log("PlayerDBをロードしました。");
        }
        else{
            Debug.LogError("PlayerDB.jsonがありませんでした。");
        }
    }
    //IDでデータを検索
    public static PlayerStat GetPlayerStatByID(int id)
    {
        if(playerDataDict.TryGetValue(id, out PlayerStat stat))
        {
            return stat.Clone();
        }
        Debug.LogError($"{id}に相当するプレイヤデータはありません。");
        return null;
    }

    //現在選択されているプレイヤを変更
    public static void SetCurrentPlayerID(int id)
    {
        currentPlayerID = id;
    }
}

