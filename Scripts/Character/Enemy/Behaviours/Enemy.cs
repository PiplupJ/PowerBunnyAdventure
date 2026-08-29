using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

//敵の状態
public enum EnemyState
{
    Active, Dead
}
//敵の初期化に必要な情報
public struct EnemyInitContext
{
    public EnemyData enemyData; //敵ステータス
    public IMapCollision mapCollision; //参照用マップ情報
    public IEnemyEntityUpdater updater; //管理者クラスに弾丸生成を報告
    public Transform target; //ターゲット(プレイヤーの座標)
}
//抽象クラスで継承用
public abstract class Enemy : PoolableObject
{
    public HealthBar healthBar; //体力バーUI用
    public IMapCollision _mapCollision;
    public IEnemyEntityUpdater _enemyUpdater;

    public EnemyData stat;

    public Vector3 moveDir; //移動方向

    public Transform target;

    protected EnemyState state;

    [SerializeField] protected List<EnemyBehaviour> _behaviours; //行動リスト
    protected EnemyBehaviour _currentBehaviour; //現在実行している行動
    protected EnemyBehaviour _nextBehaviour; //次の行動

    //モーション制御
   [SerializeField] public EnemyAnimationController animationController;

    //死亡した時実行するデリゲート
    public event Action OnDeath;

    //ダメージ数値が生成される位置
    [SerializeField] private Transform _damageAnchor;
    public Vector3 DamageAnchorPos => _damageAnchor.position;

    //隠しているか
    public bool IsStealthed = false;
    
    //体力バーを設定
    protected virtual void Awake()
    {
        if(healthBar == null){
            healthBar = GetComponent<HealthBar>();
        }
        healthBar.Init();
        //行動を優先度が高い順に整列する
        _behaviours.Sort((a,b)=> b.Priority.CompareTo(a.Priority));
        
        animationController.BindDeathEvent(this);
    }
    //初期化
    public virtual void InitStat(EnemyInitContext eic)
    {
        stat = eic.enemyData;
        _mapCollision = eic.mapCollision;
        _enemyUpdater = eic.updater;
        target = eic.target;

        stat.currentHP = stat.maxHP;
        healthBar.UpdateHealthBar(GetHpRatio());

        this.state = EnemyState.Active;

        foreach(var b in _behaviours)
        {
            b.Init(this);
        }
    }

    //次の行動を考える
    public void ProcessAI(float deltaTime)
    {
        if(this.state!=EnemyState.Active) return; 

        healthBar.UpdateRotation();
        if(_currentBehaviour == null || _currentBehaviour.CanShift)
        {
            foreach(var b in _behaviours)
            {
                if(b.CanExecute())
                {
                    _nextBehaviour = b;
                    break;
                }
            }
        }
    }
    //考えた行動をする
    public void HandleEnemyAction(float fixedDeltaTime)
    {
        if(_nextBehaviour != _currentBehaviour)
        {
            _currentBehaviour?.OnExit();
            _currentBehaviour = _nextBehaviour;
            _currentBehaviour?.OnEnter();
        }
        _currentBehaviour?.Execute(fixedDeltaTime);
    }
    
    //ダメージ処理
    public virtual void OnHit(int damage, bool wasCritical)
    {
        stat.currentHP -= damage;

        DamageLabelSpawner.ShowDamage(damage, wasCritical, DamageAnchorPos);

        //体力バーを更新
        healthBar.UpdateHealthBar(GetHpRatio());

        //後でUI関連処理を追加すること

        //体力がないなら死亡
        if(stat.currentHP<=0)
        {
            OnDie();
        }
    }

    //死亡
    public virtual void OnDie()
    {
        this.state = EnemyState.Dead;
        //_enemyUpdater.RemoveEnemy(this);

        _currentBehaviour?.OnExit();
        _currentBehaviour = null;
        _nextBehaviour = null;

        StartCoroutine(DeathRoutine());
        //ReturnToPool();]
        //DeathBehaviourの後に実行
    }
    //死亡処理
    private IEnumerator DeathRoutine()
    {
        OnDeath?.Invoke();

        yield return new WaitUntil(() => animationController.GetNormalizedTime("Death")>=1.0f);

        //エフェクト生成
        HitEffect death = ObjectPool.Instance.GetObject<HitEffect>(IDRegistry.EFFECT_DEATH);
        death.transform.position = this.transform.position;
        //敵リストから自分を除外
        _enemyUpdater.RemoveEnemy(this);
        //経験値アイテム生成
        _enemyUpdater.CreateExpItem(this.transform.position, stat.rewardEXP);
        //プールに戻る
        ReturnToPool();
    }
    //無効化されたら、念のためにコルーチンを終了
    protected void OnDisable() 
    {
        IsStealthed = false;
        StopAllCoroutines();
    }
    //残り体力の割合を返却
    public float GetHpRatio()
    {
        return (float)stat.currentHP/stat.maxHP;
    }
    //ターゲットを向いて回転
    public void RotateToTarget(Vector2 rotDir)
    {
        if(rotDir.x == 0 && rotDir.y == 0) { return; }

        transform.rotation = MovementHelper.GetRoation(rotDir);
    }
    //現在の状態(生存・死亡)を返却
    public EnemyState GetState()
    {
        return this.state;
    }

}
