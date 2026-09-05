using UnityEngine;
using System;
using System.Collections;

public class UndergroundAttackBehaviour : EnemyBehaviour
{
    protected Shot _hitBox;
    [SerializeField] private int _hitboxID;
    [SerializeField] private float activeFrame; //攻撃開始フレーム
    [SerializeField] private float recoverFrame; //攻撃終了フレーム
    [SerializeField] private float attackInterval = 1.0f;

    [SerializeField] protected float _activationRange = 10f;
    [SerializeField] protected float _stopRange = -1f;
    protected float StopRange => _stopRange > 0f ? _stopRange : _enemy.stat.attackDist;
    [SerializeField] protected float chaseSpeed = 7f;
    
    private enum AttackPhase { Dig, Move, Delay, Attack, Active, Recover, Done }
    private AttackPhase _phase;

    [SerializeField] private string digAnimation;
    [SerializeField] private string undergroundAnimation;
    [SerializeField] private string attackAnimation;
    [SerializeField] private string returnAnimation;

    [SerializeField] private GameObject dustEffect;

    private bool CanAttack;

    [SerializeField] protected float _moveRadScale   = 1.0f;
    protected float moveRad;

    private Vector3 _lastSafePosition;

    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        CanShift = true;
        CanAttack = true;
        _phase    = AttackPhase.Dig;
        _hitBox   = null;
        dustEffect.SetActive(false);
        
        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanAttack&&!DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, _activationRange);
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case AttackPhase.Dig :
                if(t >= 1.0f)
                {
                    _enemy.animationController.PlayAction(undergroundAnimation);
                    dustEffect.SetActive(true);
                    _phase = AttackPhase.Move;
                }
                break;
            case AttackPhase.Move :
                if(DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, StopRange))
                {
                    dustEffect.SetActive(false);
                    _enemy.animationController.PlayAction(attackAnimation);
                    _phase = AttackPhase.Attack;
                }
                else
                {
                    OnMove(fixedDeltaTime);
                }
                break;
            case AttackPhase.Attack :
                if(t >= activeFrame)
                {
                    SpawnHitBox();
                    _phase = AttackPhase.Active;
                }
                break;
            case AttackPhase.Active :
                if(t >= recoverFrame)
                {
                    ReleaseHitBox();
                    _phase = AttackPhase.Recover;
                }
                break;
            case AttackPhase.Recover :
                if(t >= 1.0f)
                {
                    _enemy.transform.position = _lastSafePosition;
                    _enemy.animationController.PlayAction(returnAnimation);
                    _phase = AttackPhase.Done;
                }
                break;
            case AttackPhase.Done :
                if(t >= 1.0f)
                {
                    _enemy.IsStealthed = false;
                    StartCoroutine(AttackCooldownRoutine(attackInterval/_enemy.stat.attackSpeed));
                    OnExit();
                }   
                break;
        }
    }

    public override void OnEnter()
    {   
        Debug.Log("UndergroundAttack");
        CanShift = false;
        _phase = AttackPhase.Dig;
        _hitBox = null;

        _enemy.IsStealthed = true;

        moveRad = _enemy.stat.rad * _moveRadScale;

        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(dir.x, dir.z));
        _enemy.animationController.PlayAction(digAnimation);

        _lastSafePosition = _enemy.transform.position;
    }

    public void SpawnHitBox()
    {
        _hitBox = _enemy._enemyUpdater.CreateEnemyHitBox(_hitboxID, _enemy.transform.position, _enemy.stat.attack);
    }

    public void ReleaseHitBox()
    {
        if (_hitBox == null) return;
        _hitBox.OnDie();
        _hitBox = null;
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

    private void OnMove(float fixedDeltaTime)
    {
        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;
        Vector2 moveVec = new Vector2(dir.x, dir.z) * chaseSpeed * fixedDeltaTime;
        Vector2 checkedMoveVec = MovementHelper.CheckMove(transform.position, moveVec, moveRad, _enemy._mapCollision);

        _enemy.RotateToTarget(moveVec.normalized);
        _enemy.transform.position += new Vector3(moveVec.x, 0.0f, moveVec.y); 

        float moveDist = moveVec.x * moveVec.x + moveVec.y * moveVec.y;
        float safeMoveDist = checkedMoveVec.x * checkedMoveVec.x + checkedMoveVec.y * checkedMoveVec.y;

        if(safeMoveDist >= moveDist * 0.9f)
        {
            _lastSafePosition = _enemy.transform.position;
        }        
    }

}
