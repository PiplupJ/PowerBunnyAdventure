using UnityEngine;
using System.Collections.Generic;

public abstract class Shot : PoolableObject
{
    
    //攻撃されたリスト。同じ攻撃に重複されて攻撃されることを防ぐため
    protected HashSet<int> alreadyHit = new HashSet<int>(); 

    public ShotData shotData; //ステータス情報
    public int attack; //基本プレイヤの数値によって決定
    public Vector3 moveDirection; //移動向き

    protected IMapCollision _mapCollision;
    protected IShotManager _shotManager;
    protected bool _isPlayerShot;

    public Vector3 prevPos;

    //初期化
    public void SetShotData(ShotData newData, IMapCollision mapCollision, IShotManager manager, bool isPlayerShot)
    {
        shotData = newData;
        _mapCollision = mapCollision;
        _shotManager = manager;
        _isPlayerShot = isPlayerShot;
    }
    //弾丸を発射
    public virtual void FireShot(int shooterAttack, Vector3 shotDir)
    {
        this.attack = shooterAttack;
        this.moveDirection = shotDir;
        this.transform.rotation = MovementHelper.GetRoation(new Vector2(shotDir.x, shotDir.z));
    } 
    //攻撃できる対象かを確認
    public virtual bool TryHit(int instanceID)
    {
        if(alreadyHit.Contains(instanceID)) { return false; }
        alreadyHit.Add(instanceID);
        return true;
    }
    //弾ではなく、ヒットボックスとして初期化
    public void InitAsHitBox(int shooterAttack)
    {
        this.attack = shooterAttack;
        this.moveDirection = Vector3.zero;
        shotData.moveSpeed = 0.0f;
    }

    
    //弾丸の行動。EntityManagerが呼び出す
    public abstract void HandleShotAction(float deltaTime);

    //攻撃時呼び出す。エフェクトや効果を発動
    public abstract void OnAttack(); 
    
    protected void SpawnHitEffect(Vector3 pos)
    {
        if(ObjectPool.Instance.TryGetObject<HitEffect>(shotData.hitEffectID, out HitEffect effect))
        {
            effect.transform.position = pos;
        }
    }
    //弾丸が消える時実行
    public void OnDie()
    {
        if(_isPlayerShot){
            _shotManager.RemovePlayerShot(this);
        }
        else{
            _shotManager.RemoveEnemyShot(this);
        }
        alreadyHit.Clear();
        ReturnToPool();
    }

}
