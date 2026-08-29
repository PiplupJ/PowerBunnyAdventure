using UnityEngine;

public class HitBoxShot : Shot
{
    public override void HandleShotAction(float deltaTime)
    {

    }

    public override void OnAttack()
    {
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        OnDie();
    }
}
