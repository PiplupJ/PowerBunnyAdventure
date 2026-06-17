using UnityEngine;
using System.Collections.Generic;

public class AddNewShot : AbilityCard
{
    [SerializeField] private int shotID;
    [SerializeField] private int attackBoost;

    public override void ApplyEffect(Player player)
    {
        if(player.stat.ShotIDs.Contains(shotID))
        {
            player.stat.attack.level += attackBoost;
        }
        else
        {
            player.stat.ShotIDs.Add(shotID);
        }
        ReturnToPool();
    }
}
