using UnityEngine;

public class CriticalBoost : AbilityCard
{
    [SerializeField] private int increasement;

    public override void ApplyEffect(Player player)
    {
        player.stat.criticalRate.level+= increasement;

        ReturnToPool();
    }
}
