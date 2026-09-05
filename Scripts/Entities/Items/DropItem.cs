using UnityEngine;

public abstract class DropItem : PoolableObject
{
    public float rad; 

    public IDropItemSystem itemSystem;

    public IPlayerReciever receiver;
    
    public void Init(IDropItemSystem dropItemSystem, IPlayerReciever playerReceiver)
    {
        itemSystem = dropItemSystem;
        receiver = playerReceiver;
    }
    public abstract void HandleItemAction(float deltaTime);
    public abstract void ApplyEffect();

    public void OnUse()
    {
        itemSystem.RemoveDropItem(this);
        ReturnToPool();
    }
}
