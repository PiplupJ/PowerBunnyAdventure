using UnityEngine;

public class KiteBehaviour : EnemyBehaviour
{
    [SerializeField] private float _probeAngle   = 45f; 
    [SerializeField] private int   _probeCount   = 8;  
    [SerializeField] protected float speed = 0.5f;
    private float   _kitingDistance = 30;
    Vector2 moveDir;

    public override bool CanExecute()
    {
        return DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, _kitingDistance);
    }

    public override void OnEnter()
    {
        float angle = _probeAngle * Random.Range(0, _probeCount) * Mathf.Deg2Rad;

        Vector2 vec = new Vector2(1,1);

        moveDir = new Vector2(
            vec.x * Mathf.Cos(angle) - vec.y * Mathf.Sin(angle),
            vec.x * Mathf.Sin(angle) + vec.y * Mathf.Cos(angle));
    }

    public override void Execute(float fixedDeltaTime)
    {
        OnKite(fixedDeltaTime);
    }

    public override void OnExit()
    {

    }

    public virtual void OnKite(float fixedDeltaTime)
    {
        _enemy.animationController.UpdateBlendMotion(speed);
        
        Vector2 moveVec = new Vector2(moveDir.x, moveDir.y) * _enemy.stat.moveSpeed * fixedDeltaTime;

        moveVec = MovementHelper.CheckMove(transform.position, moveVec, _enemy.stat.rad, _enemy._mapCollision);

        _enemy.RotateToTarget(moveVec.normalized);
        _enemy.transform.position += new Vector3(moveVec.x, 0.0f, moveVec.y);
    }
}
