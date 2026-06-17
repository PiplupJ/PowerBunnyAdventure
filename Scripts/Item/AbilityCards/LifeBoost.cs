using UnityEngine;

public class LifeBoost : AbilityCard
{
    public override void ApplyEffect(Player player)
    {
        player.stat.resurrectCount += 1;

        ReturnToPool();
    }
}
