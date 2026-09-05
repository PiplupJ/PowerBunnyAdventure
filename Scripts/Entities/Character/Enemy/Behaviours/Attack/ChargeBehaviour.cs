using UnityEngine;
using System.Collections;

public class ChargeBehaviour : EnemyBehaviour
{
    protected enum ChargingPhase { WindUp, Charging, Recover }
    protected ChargingPhase _phase;
    protected Vector3 chargeDirection;
    [SerializeField] protected float chargeSpeed = 10.0f;
    [SerializeField] protected float chargeDistance = 10.0f;
    [SerializeField] protected float minChargeTime = 1.0f;
    [SerializeField] protected float maxChargeTime = 3.0f;
    protected float chargeTimer; 

    protected Shot _hitBox;
    [SerializeField] protected int _hitboxID;
    [SerializeField] protected string windUpAnimation;
    [SerializeField] protected string chargeAnimation;
    [SerializeField] protected string recoverAnimation;
    [SerializeField] protected float attackInterval = 3.0f;

    [SerializeField] protected float _moveRadScale   = 1.0f;
    protected float moveRad;

    protected bool CanAttack;

    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        CanShift = true;
        CanAttack = true;
        _phase    = ChargingPhase.WindUp;
        _hitBox   = null;
        
        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanAttack && DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, chargeDistance);
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case ChargingPhase.WindUp :
                if(t >= 1.0f)
                {
                    _enemy.animationController.PlayAction(chargeAnimation);
                    OnChargeStart();
                    _phase = ChargingPhase.Charging;
                }
                break;
            case ChargingPhase.Charging :
                TryCharge(fixedDeltaTime);
                chargeTimer += fixedDeltaTime;
                if(chargeTimer >= maxChargeTime)
                {
                    _enemy.animationController.PlayAction(recoverAnimation);
                    ReleaseHitBox();
                    _phase = ChargingPhase.Recover;
                }
                break;
            case ChargingPhase.Recover :
                if(t >= 1.0f)
                {
                    StartCoroutine(AttackCooldownRoutine(attackInterval/_enemy.stat.attackSpeed));
                    OnExit();
                }
                break;
        }
    }

    protected void TryCharge(float fixedDeltaTime)
    {
        Vector2 moveVec = new Vector2(chargeDirection.x, chargeDirection.z) * chargeSpeed * fixedDeltaTime;
        MoveResult res = MovementHelper.CheckMoveInDetail(_enemy.transform.position, moveVec, moveRad, _enemy._mapCollision);
        
        _enemy.transform.position += new Vector3(res.moveVec.x, 0.0f, res.moveVec.y);

        if(res.hitwallX || res.hitwallZ)
        {
            if(chargeTimer >= minChargeTime)
            {
                _enemy.animationController.PlayAction(recoverAnimation);
                ReleaseHitBox();
                _phase = ChargingPhase.Recover;
            }
        }
    }

    protected virtual void OnChargeStart()
    {
        SpawnHitBox();
    }

    public void SpawnHitBox()
    {
        _hitBox = _enemy._enemyUpdater.CreateEnemyHitBox(_hitboxID, _enemy.transform.position, _enemy.stat.attack);

        if(_hitBox is FollowShot followShot)
        {
            followShot.SetFollowTarget(_enemy.transform);
        }
    }

    public void ReleaseHitBox()
    {
        if (_hitBox == null) return;
        _hitBox.OnDie();
        _hitBox = null;
    }

    public override void OnEnter()
    {   
        moveRad = _enemy.stat.rad * _moveRadScale;

        CanShift = false;
        _phase = ChargingPhase.WindUp;
        _hitBox = null;

        chargeTimer = 0.0f;
        chargeDirection = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(chargeDirection.x, chargeDirection.z));
        _enemy.animationController.PlayAction(windUpAnimation);
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
}
