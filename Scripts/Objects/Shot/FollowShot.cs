using UnityEngine;

public class FollowShot : Shot
{
    private Transform _followTarget;

    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(_followTarget == null) return;

        transform.position = _followTarget.position;
    }

    public override void OnAttack()
    {
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        OnDie();
    }
}
