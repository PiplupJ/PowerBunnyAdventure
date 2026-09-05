using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum StatType
{
    Attack, Defense, Health, AttackSpeed, MoveSpeed, Critical
}

[System.Serializable]
public class StatBoostEntry
{
    public StatType statType;  
    public int      rankBoost; 
}
public class StatBoostCard : AbilityCard
{
    //このカードで得られるステータスバフをインスペクターで作成
    [Tooltip("必要な分のステータスブーストを入れてください")]
    [SerializeField] private List<StatBoostEntry> _statBoosts;

    //カードの効果実行
    public override void ApplyEffect(Player player)
    {
        foreach(var boost in _statBoosts)
        {
            player.ApplyStatBoost(boost.statType, boost.rankBoost);
        }
        ReturnToPool();
    }
}
