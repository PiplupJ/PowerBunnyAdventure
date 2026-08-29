using UnityEngine;
using System.Collections.Generic;

public class AddNewShot : AbilityCard
{
    [SerializeField] private int shotID;
    [SerializeField] private int attackBoost;

    //カードの効果実行
    public override void ApplyEffect(Player player)
    {
        //プレイヤーが同じ種類の弾を持っていると代わりに攻撃増加
        if(player.stat.ShotIDs.Contains(shotID))
        {
            player.stat.attack.level += attackBoost;
        }
        else //ないならプレイヤーの弾リストに弾を追加
        {
            player.stat.ShotIDs.Add(shotID);
        }
        ReturnToPool();
    }
}
