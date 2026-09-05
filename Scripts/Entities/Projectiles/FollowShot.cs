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
        SpawnHitEffect(new Vector3(transform.position.x, 1, transform.position.z));
        OnDie();
    }
}
