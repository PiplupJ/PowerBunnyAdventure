using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public enum EnemyState
{
    Active, Dead
}
public struct EnemyInitContext
{
    public EnemyData enemyData;
    public IMapCollision mapCollision;
    public IEnemyEntityUpdater updater;
    public Transform target;
}
//抽象クラスで継承用
public abstract class Enemy : PoolableObject
{
    public HealthBar healthBar; //体力バーUI用
    public IMapCollision _mapCollision;
    public IEnemyEntityUpdater _enemyUpdater;

    public EnemyData stat;

    public Vector3 moveDir;

    public Transform target;

    protected EnemyState state;

    [SerializeField] protected List<EnemyBehaviour> _behaviours;
    protected EnemyBehaviour _currentBehaviour;
    protected EnemyBehaviour _nextBehaviour;

   [SerializeField] public EnemyAnimationController animationController;

   public event Action OnDeath;

    [SerializeField] private Transform _damageAnchor;
    public Vector3 DamageAnchorPos => _damageAnchor.position;

    public bool IsStealthed = false;
    
    //体力バーを設定
    protected virtual void Awake()
    {
        if(healthBar == null){
            healthBar = GetComponent<HealthBar>();
        }
        healthBar.Init();
        _behaviours.Sort((a,b)=> b.Priority.CompareTo(a.Priority));
        
        animationController.BindDeathEvent(this);
    }

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

    private IEnumerator DeathRoutine()
    {
        OnDeath?.Invoke();

        yield return new WaitUntil(() => animationController.GetNormalizedTime("Death")>=1.0f);

        HitEffect death = ObjectPool.Instance.GetObject<HitEffect>(IDRegistry.EFFECT_DEATH);
        death.transform.position = this.transform.position;
        _enemyUpdater.RemoveEnemy(this);//0619追加
        _enemyUpdater.CreateExpItem(this.transform.position, stat.rewardEXP);
        ReturnToPool();
    }

    protected void OnDisable() 
    {
        IsStealthed = false;
        StopAllCoroutines();
    }

    public float GetHpRatio()
    {
        return (float)stat.currentHP/stat.maxHP;
    }

    public void RotateToTarget(Vector2 rotDir)
    {
        if(rotDir.x == 0 && rotDir.y == 0) { return; }

        transform.rotation = MovementHelper.GetRoation(rotDir);
    }

    public EnemyState GetState()
    {
        return this.state;
    }

}
