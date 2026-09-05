using UnityEngine;

public class HitBoxShot : Shot
{
    public override void HandleShotAction(float deltaTime)
    {

    }

    public override void OnAttack()
    {
        SpawnHitEffect(new Vector3(transform.position.x, 1, transform.position.z));
        OnDie();
    }
}
