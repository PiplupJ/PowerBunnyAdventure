using UnityEngine;

public class SplitShot : Shot
{
    [SerializeField] private float splitTimer = 1.5f;
    [SerializeField] private int splitCount;
    [SerializeField] private int shotID;
    private float timer = 0.0f;

    private void OnEnable() 
    {
        timer = splitTimer;    
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(timer <= 0)
        {
            Split();
        }

        if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            Split();
        }
        prevPos = transform.position;

        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;

        timer -= deltaTime;
    }

    public override void OnAttack()
    {
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        OnDie();
    }

    private void Split()
    {
        float baseAngle = Mathf.Atan2(moveDirection.x, moveDirection.z);

        for(int i = 0; i < splitCount; i++)
        {
            float angle = i * (360f / splitCount) * Mathf.Deg2Rad;
            float finalAngle = baseAngle + angle;
            Vector3 dir = new Vector3(Mathf.Cos(finalAngle), 0f, Mathf.Sin(finalAngle));

            if(_isPlayerShot){
            _shotManager.CreatePlayerShot(shotID, transform.position, attack, dir);
            }
            else{
                _shotManager.CreateEnemyShot(shotID, transform.position, attack, dir);
            }
        }
        OnDie();
    }

}
