using UnityEngine;

public class HealingCard : AbilityCard
{
    [SerializeField] private float healingRatio = 0.5f;

    public override void ApplyEffect(Player player)
    {
        int healAmount = (int)(player.MaxHP * healingRatio);
        player.OnHeal(healAmount);
        ReturnToPool();
    }
}
