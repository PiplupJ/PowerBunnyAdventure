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
        player.AddShot(shotID, attackBoost);
        ReturnToPool();
    }
}
