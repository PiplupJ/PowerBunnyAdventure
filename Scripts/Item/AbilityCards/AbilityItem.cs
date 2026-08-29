using UnityEngine;

//ID 7000000~7999999 
//プレイヤのステータスに影響を与えるアイテム
//継承して使用すること
public abstract class AbilityItem : PoolableObject
{
    //プレイヤを引数に求め、ステータス増加効果を適用
    public abstract void ApplyEffect(Player currentPlayer);
}
