/*
時間が経ったらハザードを生成する弾
*/
using UnityEngine;

public class HazardShot : Shot
{   
    [SerializeField] private float bombTimer = 1.5f; //発射からハザード生成までの時間
    [SerializeField] private int hazardID; //生成する弾のID

    private float timer = 0.0f;

    private void OnEnable() 
    {
        timer = bombTimer;  //タイマー初期化
    }

    public override void HandleShotAction(float deltaTime)
    {   
        //時間が経ったらハザード生成
        if(timer <= 0)
        {
            CreateHazard()
;       }
        else if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            //または壁に衝突したらハザード生成
            CreateHazard();
        }
        prevPos = transform.position;
        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;

        timer -= deltaTime;
    }

    //キャラクターに衝突したら、弾として攻撃処理
    public override void OnAttack()
    {
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        OnDie();
    }
    //ハザード生成
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
