using UnityEngine;

//爆発する弾
public class ExplosionShot : Shot
{
    [SerializeField] private int explosionID;

    public override void HandleShotAction(float deltaTime)
    {
        if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            Explosion();
            return;
        }
        prevPos = transform.position;
        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;
    }

    public override void OnAttack()
    {
        Explosion();
        return;
    }

    private void Explosion()
    {
        if(_isPlayerShot){
            _shotManager.CreatePlayerShot(explosionID, transform.position, attack, moveDirection);
        }
        else{
            _shotManager.CreateEnemyShot(explosionID, transform.position, attack, moveDirection);
        }
        OnDie();
    }
}
