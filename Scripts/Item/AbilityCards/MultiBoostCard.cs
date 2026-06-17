using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum StatType
{
    Attack, Defense, Health, AttackSpeed, MoveSpeed
}

[System.Serializable]
public class StatBoostEntry
{
    public StatType statType;  
    public int      rankBoost; 
}
public class MultiBoostCard : AbilityCard
{
    [SerializeField] private List<StatBoostEntry> _statBoosts;

    public override void ApplyEffect(Player player)
    {
        
        foreach(var boost in _statBoosts)
        {
            ApplyStatBoost(player, boost);
        }
        ReturnToPool();
    }

    private void ApplyStatBoost(Player player, StatBoostEntry boost)
    {
        switch(boost.statType)
        {
            case StatType.Attack :
                player.stat.attack.level+= boost.rankBoost;
                break;
            case StatType.Defense :
                player.stat.defense.level+= boost.rankBoost;
                break;
            case StatType.Health :
                player.stat.hp.level+= boost.rankBoost;
                player.OnHeal(0);
                break;
            case StatType.AttackSpeed :
                player.stat.attackSpeed.level+= boost.rankBoost;
                break;
            case StatType.MoveSpeed :
                player.stat.moveSpeed.level+= boost.rankBoost;
                break;
            default :
                break;
        }
    }
}
