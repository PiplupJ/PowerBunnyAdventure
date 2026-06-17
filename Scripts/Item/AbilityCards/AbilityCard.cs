using UnityEngine;
using System.Collections.Generic;
//jsonファイル作成用
//カードのデータはファイルで保管
//効果を使う時のみObjectPoolから呼び出す

//カードのレアリティ。Epic>Rare>Normal順に登場確率が低い
public enum CardRarity { Normal, Rare, Epic }

[System.Serializable]
public class AbilityCardRawData
{
    //カードのid.ObjectPoolのIDと一致すること
    public int id;
    //カードの名前
    public string name;
    //イメージファイルの保存先。Resources/Data/Image/AbilityCardsに保存。
    //ファイル名はカードの名前にすること
    //例）"Data/AbilityCards/Image/ファイル名"の形式に書く
    //Resources.Load<Sprite>(abilityCardData.cardImagePath);
    //を使ってスプライトイメージかすること
    public string cardImagePath;
    //カードの説明
    public string description;
    //カードのレアリティ
    public CardRarity rarity;
}
[System.Serializable]
public class AbilityCardRawDataLists
{
    //カードをレアリティ別に分類
    public List<AbilityCardRawData> normalCards = new List<AbilityCardRawData>();
    public List<AbilityCardRawData> rareCards = new List<AbilityCardRawData>();
    public List<AbilityCardRawData> epicCards = new List<AbilityCardRawData>();
}
//継承用クラス。
//これを継承してカードの効果を持っているカード別のクラスを作成
//カード効果を適用する時、ObjectPoolからもらって、中身のApplyEffect(Player currentPlayer)関数を呼び出す。
public abstract class AbilityCard : AbilityItem
{
    
}
