using UnityEngine;

public class BasicShot : Shot
{
    public override void HandleShotAction(float deltaTime)
    {
        if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            OnDie();
            return;
        }
        prevPos = transform.position;
        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;
    }

    public override void OnAttack()
    {
        SpawnHitEffect(new Vector3(transform.position.x, 1, transform.position.z));
        OnDie();
    }
}
