using UnityEngine;
using System;

public class ChaseBehaviour : EnemyBehaviour
{
    [SerializeField] protected float _stopRange = -1f;
    protected float StopRange => _stopRange > 0f ? _stopRange : _enemy.stat.attackDist;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected float _moveRadScale   = 1.0f;
    protected float moveRad;

    public override bool CanExecute()
    {
        return !DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, StopRange);
    }

    public override void Execute(float fixedDeltaTime)
    {   
        OnChase(fixedDeltaTime);
    }

    public virtual void OnChase(float fixedDeltaTime)
    {
        _enemy.animationController.UpdateBlendMotion(speed);
        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;
        Vector2 moveVec = new Vector2(dir.x, dir.z) * _enemy.stat.moveSpeed * fixedDeltaTime;

        moveVec = MovementHelper.CheckMove(transform.position, moveVec, moveRad, _enemy._mapCollision);

        _enemy.RotateToTarget(moveVec.normalized);
        _enemy.transform.position += new Vector3(moveVec.x, 0.0f, moveVec.y);
    }

    public override void OnEnter()
    {
        moveRad = _enemy.stat.rad * _moveRadScale;
        base.OnEnter();
    }
}
