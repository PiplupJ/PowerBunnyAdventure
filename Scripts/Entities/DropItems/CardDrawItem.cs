using UnityEngine;

public class CardDrawItem : DropItem
{
    [SerializeField] private CardRarity rarity = CardRarity.Epic;

    public override void HandleItemAction(float deltaTime)
    {

    }
    public override void ApplyEffect()
    {
        GameManager.Instance.StartCardDrawEventWithRarity(rarity);
        itemSystem.RemoveDropItem(this);
        ReturnToPool();
    }
}
