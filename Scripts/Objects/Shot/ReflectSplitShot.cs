using UnityEngine;

//壁にぶつかったら移動向きが変わる弾
public class ReflectSplitShot : Shot
{
    [SerializeField] private int _childShotID;
    [SerializeField] private float attackDivide = 0.5f;

    public override void HandleShotAction(float deltaTime)
    {
        prevPos = transform.position;

        Vector3 nextPos = transform.position + 
                         moveDirection * shotData.moveSpeed * deltaTime;

        MoveResult result = MovementHelper.CheckMoveInDetail(
            transform.position,
            new Vector2(moveDirection.x, moveDirection.z) * shotData.moveSpeed * deltaTime,
            shotData.rad,
            _mapCollision);

        transform.position += new Vector3(result.moveVec.x, 0f, result.moveVec.y);

        if (result.hitwallX || result.hitwallZ)
        {
            SpawnReflectedShots(result.hitwallX, result.hitwallZ);
            OnDie();
        }
    }

    private void SpawnReflectedShots(bool hitX, bool hitZ)
    {
        //元の向きの反対側
        Vector3 oppositeDir = -moveDirection;

        Vector3 reflectedDir = moveDirection;
        if (hitX) reflectedDir.x = -reflectedDir.x; //X軸衝突
        if (hitZ) reflectedDir.z = -reflectedDir.z; // Z軸衝突

        SpawnChildShot(oppositeDir);
        SpawnChildShot(reflectedDir);
    }

    private void SpawnChildShot(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        int childShotAttack = (int)(attack * attackDivide);

        if(_isPlayerShot){
            _shotManager.CreatePlayerShot(_childShotID, transform.position, childShotAttack, dir);
            }
            else{
                _shotManager.CreateEnemyShot(_childShotID, transform.position, childShotAttack, dir);
            }
    }

    public override void OnAttack()
    {
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        OnDie();
    }

}
