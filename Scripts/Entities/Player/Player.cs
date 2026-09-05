using UnityEngine;
using System.Collections;
using System;

public interface IPlayerControl
{
    void HandlePlayerInput(Vector2 newMoveDir);
    void HandlePlayerAction(float deltaTime);

    Vector3 playerPos { get; }
    Transform playerRoot { get;}
}

public interface IPlayerReciever
{
    void OnHit(int damage, bool wasCrit);
    void OnHeal(int healAmount);
    int MaxHP { get; }
    Vector3 playerPos { get; }
}

public class Player : PoolableObject, IPlayerControl, IStatusReceiver, IPlayerReciever
{
    public IMapCollision mapCollision; //マップとの当たり判定のため必要
    public IPlayerCombatHelper combatHelper; //弾丸を登録するため必要
    public ILevelUpSystem levelUpSystem;
    public HealthBar healthBar;

    public PlayerStat stat { get; private set;}
    protected bool CanAttack;
    protected Vector2 moveDirection;
    public Vector3 playerPos => transform.position;
    public Transform playerRoot => transform;

    public event Action PlayerIsDead;
    public event Action OnHpUpdate;

    public event Action IsAttacking;

    public PlayerAnimationController animationController;

    //public StatusSystem statusSystem;
    public int MaxHP => stat.hp.maxHp;

    [SerializeField] private Transform damageAnchor;
    public Vector3 DamageAnchorPos => damageAnchor.position;

    //初期化
    public void Init(int id, IMapCollision mapCollision, IPlayerCombatHelper combatHelper, ILevelUpSystem levelUpSystem)
    {   
        //ファイルからステータスデータを求める
        stat = PlayerDataManager.GetPlayerStatByID(id);
        stat.InitStats();
        this.mapCollision = mapCollision;
        this.combatHelper = combatHelper;
        this.levelUpSystem = levelUpSystem;
        CanAttack = true;
        if(healthBar==null){
            healthBar = GetComponent<HealthBar>();
        } 
        healthBar.Init();
        this.transform.position = Vector3.zero;

        animationController.BindToMotion(this);
    }

    //更新。移動入力があったら移動、なかったら攻撃。
    //PlayerControllerの更新で呼び出す。
    public void HandlePlayerInput(Vector2 newMoveDir)
    {
        moveDirection = newMoveDir;
    }
    public void HandlePlayerAction(float deltaTime)
    {
        if(moveDirection.x != 0 || moveDirection.y != 0)
        {
            PlayerMove(moveDirection, deltaTime);
        }
        else{
            animationController.UpdateBlendMotion(0.0f);
            PlayerAttack();
        }
        healthBar.UpdateRotation();
    }

    //プレイヤの移動処理。移動量を計算して、移動後の座標を求める
    protected void PlayerMove(Vector2 moveDir, float deltaTime)
    {
        Vector2 moveVec = moveDir * deltaTime * stat.moveSpeed.value; //移動量計算
        
        animationController.UpdateBlendMotion(1.0f);
        
        CheckMove(moveVec); //移動が正しいか
        mapCollision.HitTileCheck(this.transform.position, stat.rad);
    }

    //マップとの当たり判定を処理して、座標をアップデート
    protected void CheckMove(Vector2 moveVec)
    {
        Vector2 finalMoveValue = MovementHelper.CheckMove(transform.position, moveVec, stat.rad, mapCollision);

        this.transform.position += new Vector3(finalMoveValue.x, 0.0f, finalMoveValue.y);

        //回転処理
        PlayerRotateTowardsTarget(moveVec);
    }

    protected void PlayerAttack()
    {   
        //攻撃できない状態なら攻撃しない
        if(!CanAttack) { return ;}
        
        if(stat.ShotIDs.Count==0 || stat.ShotIDs == null) {
            //Debug.Log("プレイヤの弾丸がありません。");
            return;
        }
        //Debug.Log("現在の攻撃範囲"+stat.attackDist.value);
        Enemy target = combatHelper.GetNearestTarget(this.transform.position, stat.attackDist.value);
        //Enemy target = combatHelper.GetNearestTarget(this.transform.position, stat.attackDist.value);
        if(target == null) { 
            //Debug.Log("攻撃対象を見つかりませんでした。");
            return; 
        }
        
        Vector3 shotDir = (target.transform.position - this.transform.position).normalized;
        //回転処理
        PlayerRotateTowardsTarget(new Vector2(shotDir.x, shotDir.z));
        IsAttacking?.Invoke();
        //弾丸を発射
        for(int i = 0; i < stat.ShotIDs.Count; i++){
            //ObjectPoolに発射するShotを求める。
            combatHelper.CreatePlayerShot(stat.ShotIDs[i], this.transform.position, stat.attack.value, shotDir);
        }
        //Debug.Log("攻撃完了");
        //クールダウン開始。攻撃周期/攻撃速度待つ
        StartCoroutine(AttackCooldownRoutine(stat.attackInterval/stat.attackSpeed.value));
    }


    protected IEnumerator AttackCooldownRoutine(float coolTime)
    {
        CanAttack = false;

        yield return new WaitForSeconds(coolTime);
        CanAttack = true;
    }

    protected void PlayerRotateTowardsTarget(Vector2 rotDir)
    {
        if(rotDir.x == 0 && rotDir.y == 0) { return; }

        transform.rotation = MovementHelper.GetRoation(rotDir);
    }

    //ダメージ処理
    public void OnHit(int damage, bool wasCrit)
    {
        stat.hp.TakeDamage(damage);
        DamageLabelSpawner.ShowDamage(damage, wasCrit, DamageAnchorPos);
        healthBar.UpdateHealthBar(stat.hp.GetHpRatio());
        OnHpUpdate?.Invoke();
        if(stat.hp.currentHp<=0){
            OnDie();
        }
    }

    public void OnDie()
    {
        if(stat.resurrectCount > 0){
            PlayerResurrect();
            stat.resurrectCount--;
        }else{
            PlayerIsDead?.Invoke();
        }
    }

    public void OnHeal(int healAmount)
    {
        if(ObjectPool.Instance.TryGetObject<HitEffect>(IDRegistry.EFFECT_HEAL, out HitEffect healEffect))
        {
            healEffect.transform.position = transform.position;
        }
        stat.hp.Heal(healAmount);
        RefreshHealthUI();
    }

    public void PlayerResurrect()
    {
        if(ObjectPool.Instance.TryGetObject<HitEffect>(IDRegistry.RESURRECTION , out HitEffect resurrectEffect))
        {
            resurrectEffect.transform.position = transform.position;
        }
        stat.hp.Heal(stat.hp.maxHp);
        RefreshHealthUI();
    }

    public void ApplyStatBoost(StatType type, int rankValue)
    {
        switch(type)
        {
            case StatType.Attack:
                stat.attack.level += rankValue;
                break;
            case StatType.Defense:
                stat.defense.level += rankValue;
                break;
            case StatType.AttackSpeed:
                stat.attackSpeed.level += rankValue;
                break;
            case StatType.Critical :
                stat.criticalRate.level += rankValue;
                break;
            case StatType.MoveSpeed:
                stat.moveSpeed.level += rankValue;
                break;
            case StatType.Health:
                stat.hp.level += rankValue;
                RefreshHealthUI();  
                break;  
            default:
                break;
        }
    }

    public void AddShot(int shotID, int attackBoostIfOwned)
    {
        if(stat.ShotIDs.Contains(shotID))
        {
            stat.attack.level += attackBoostIfOwned;
        }
        else
        {
            stat.ShotIDs.Add(shotID);
        }
    }

    public void AddResurrect(int amount)
    {
        stat.resurrectCount += amount;
    }

    public void RefreshHealthUI()
    {
        healthBar.UpdateHealthBar(stat.hp.GetHpRatio());
        OnHpUpdate?.Invoke();
    }
}
