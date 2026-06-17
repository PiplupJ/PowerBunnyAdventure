using UnityEngine;

public class DefenseBoost : AbilityCard
{
    [SerializeField] private int increasement;

    public override void ApplyEffect(Player player)
    {
        player.stat.defense.level+= increasement;

        ReturnToPool();
    }
}
