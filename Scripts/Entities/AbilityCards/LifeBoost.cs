using UnityEngine;

public class LifeBoost : AbilityCard
{
    public override void ApplyEffect(Player player)
    {
        player.AddResurrect(1);

        ReturnToPool();
    }
}
