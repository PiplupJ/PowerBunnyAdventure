using UnityEngine;

public class AttackBoost : AbilityCard
{
    [SerializeField] private int increasement;

    public override void ApplyEffect(Player player)
    {
        player.stat.attack.level+= increasement;

        ReturnToPool();
    }
}
