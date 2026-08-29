using UnityEngine;

public class HealthBoost : AbilityCard
{
    [SerializeField] private int increasement;

    public override void ApplyEffect(Player player)
    {
        player.stat.hp.level+= increasement;
        player.OnHeal(0);

        ReturnToPool();
    }
}
