using UnityEngine;

public class TrickItem : DropItem
{
    [SerializeField] private int _eventID;
    [SerializeField] private int _trickEffectID;

    public override void HandleItemAction(float deltaTime)
    {
        
    }

    public override void ApplyEffect()
    {
        HitEffect trickEffect = ObjectPool.Instance.GetObject<HitEffect>(_trickEffectID);
        trickEffect.transform.position = transform.position;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        DropItem eventCharacter = itemSystem.CreateItemAtPosition(_eventID, spawnPos);

        if(eventCharacter is EventChracter evc)
        {
            evc.StartEvent();
        }

        OnUse();
    }

}
