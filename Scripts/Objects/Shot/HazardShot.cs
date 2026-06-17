using UnityEngine;

public class HazardShot : Shot
{
    [SerializeField] private float bombTimer = 1.5f;
    [SerializeField] private int hazardID;

    private float timer = 0.0f;

    private void OnEnable() 
    {
        timer = bombTimer;    
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(timer <= 0)
        {
            CreateHazard()
;       }
        else if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            CreateHazard();
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

    private void CreateHazard()
    {
        if(_isPlayerShot){
            _shotManager.CreatePlayerShot(hazardID, transform.position, attack, Vector3.zero);
        }
        else{
            _shotManager.CreateEnemyShot(hazardID, transform.position, attack, Vector3.zero);
        }
        OnDie();
    }
}
