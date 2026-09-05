using UnityEngine;

public class TrickItem : DropItem
{
    [SerializeField] private int _eventID;
    [SerializeField] private int _trickEffectID;

    public override void HandleItemAction(float deltaTime)
    {
        
    }
    //プレイヤーが範囲に入ったらイベントキャラクター生成
    public override void ApplyEffect()
    {
        if(ObjectPool.Instance.TryGetObject<HitEffect>(_trickEffectID, out HitEffect trickEffect))
        {
            trickEffect.transform.position = transform.position;
        }

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        DropItem eventCharacter = itemSystem.CreateItemAtPosition(_eventID, spawnPos);

        if(eventCharacter != null && eventCharacter is EventCharacter evc)
        {
            evc.StartEvent();
        }

        OnUse();
    }

}
