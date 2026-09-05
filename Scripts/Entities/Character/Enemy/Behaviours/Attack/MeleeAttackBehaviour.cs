using UnityEngine;
using System;
using System.Collections;

public class MeleeAttackBehaviour : EnemyBehaviour
{

    [SerializeField] private float _attackRange = -1f;
    private float AttackRange => _attackRange > 0f ? _attackRange : _enemy.stat.attackDist;

    protected Shot _hitBox;
    [SerializeField] private int _hitboxID;
    [SerializeField] private float activeFrame; //攻撃開始フレーム
    [SerializeField] private float recoverFrame; //攻撃終了フレーム
    [SerializeField] private string animationTag;
    [SerializeField] private float attackInterval = 1.0f;

    //近接攻撃の４つの段階
    private enum AttackPhase { WindUp, Active, Recover, Done }

    private AttackPhase _phase;

    private bool CanAttack;

    [SerializeField] private float firePointOffset = 0.0f;

    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        CanShift = true;
        CanAttack = true;
        _phase    = AttackPhase.WindUp;
        _hitBox   = null;
        
        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanAttack && DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, AttackRange);
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case AttackPhase.WindUp :
                if(t >= activeFrame)
                {
                    SpawnHitBox(); //ヒットボックスを生成
                    _phase = AttackPhase.Active;
                }
                break;
            case AttackPhase.Active :
                if(t >= recoverFrame)
                {
                    ReleaseHitBox(); //ヒットボックスを無効化
                    _phase = AttackPhase.Recover;
                }
                break;
            case AttackPhase.Recover :
                if(t >= 1.0f)
                {
                    _phase = AttackPhase.Done;
                }
                break;
            case AttackPhase.Done :
                StartCoroutine(AttackCooldownRoutine(attackInterval/_enemy.stat.attackSpeed));
                CanShift = true;
                break;
        }
    }

    public void SpawnHitBox()
    {
        _hitBox = _enemy._enemyUpdater.CreateEnemyHitBox(_hitboxID, GetFirePoint(), _enemy.stat.attack);
    }

    public void ReleaseHitBox()
    {
        if (_hitBox == null) return;
        _hitBox.OnDie();
        _hitBox = null;
    }

    public override void OnEnter()
    {   
        CanShift = false;
        _phase = AttackPhase.WindUp;
        _hitBox = null;

        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(dir.x, dir.z));
        _enemy.animationController.PlayAction(animationTag);
    }

    public override void OnExit()
    {   
        CanShift = true;
        ReleaseHitBox(); //念のため、攻撃終了処理
    }

    protected IEnumerator AttackCooldownRoutine(float coolTime)
    {
        CanAttack = false;

        yield return new WaitForSeconds(coolTime);
        CanAttack = true;
    }

    public Vector3 GetFirePoint()
    {
        return _enemy.transform.position + 
               _enemy.transform.forward * firePointOffset;
    }
}
