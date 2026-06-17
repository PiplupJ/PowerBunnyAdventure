using UnityEngine;

public class HealingItem : DropItem
{
    [SerializeField] private float healingRatio = 0.5f;

    public override void HandleItemAction(float deltaTime)
    {
        
    }
    public override void ApplyEffect()
    {
        int healAmount = (int)(receiver.MaxHP * healingRatio);
        receiver.OnHeal(healAmount);
        
        OnUse();
    }
}
