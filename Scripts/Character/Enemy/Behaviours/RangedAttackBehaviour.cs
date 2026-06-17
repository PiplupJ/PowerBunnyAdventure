using UnityEngine;
using System;
using System.Collections;

public class RangedAttackBehaviour : EnemyBehaviour
{
    [SerializeField] private float _attackRange = -1f;
    private float AttackRange => _attackRange > 0f ? _attackRange : _enemy.stat.attackDist;

    [SerializeField] private int _shotID;
    [SerializeField] private float shootingFrame;
    [SerializeField] private float recoverFrame;
    [SerializeField] private float attackInterval = 1.0f;
    [SerializeField] private string animationTag;

    [SerializeField] private float firePointOffset = 1.0f;

    private enum AttackPhase { WindUp, Active, Recover, Done }

    private AttackPhase _phase;

    private bool CanAttack;

    protected Vector3 shotDir;

    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        CanAttack = true;
        _phase    = AttackPhase.WindUp;
        
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
                if(t >= shootingFrame)
                {
                    SpawnShot();
                    _phase = AttackPhase.Active;
                }
                break;
            case AttackPhase.Active :
                if(t >= recoverFrame)
                {
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
                OnExit();
                break;
        }
    }

    public void SpawnShot()
    {
        _enemy._enemyUpdater.CreateEnemyShot(_shotID, GetFirePoint(), _enemy.stat.attack, shotDir);
    }

    public Vector3 GetFirePoint()
    {
        return _enemy.transform.position + 
               _enemy.transform.forward * firePointOffset;
    }

    public override void OnEnter()
    {   
        Debug.Log("Start Attack");
        CanShift = false;
        _phase = AttackPhase.WindUp;

        shotDir = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(shotDir.x, shotDir.z));
        _enemy.animationController.PlayAction(animationTag);
    }

    public override void OnExit()
    {   
        CanShift = true;
    }

    protected IEnumerator AttackCooldownRoutine(float coolTime)
    {
        CanAttack = false;

        yield return new WaitForSeconds(coolTime);
        CanAttack = true;
    }
}
