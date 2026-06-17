using UnityEngine;

public class SelfDestructBehaviour : EnemyBehaviour
{
    [SerializeField] private float _attackRange = -1f;
    private float AttackRange => _attackRange > 0f ? _attackRange : _enemy.stat.attackDist;
    [SerializeField] private int _shotID;
    [SerializeField] private string animationTag;
    bool used;
    bool CanAttack;

    public override void Init(Enemy enemy)
    {
        CanShift = true;
        CanAttack = true;
        used = false; 

        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanAttack && DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, AttackRange);
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        if(!used&&t>=1.0f)
        {
            used = true;
            SpawnShot();
            _enemy.OnHit(_enemy.stat.maxHP, true);
        }
    }

    public void SpawnShot()
    {
        _enemy._enemyUpdater.CreateEnemyShot(_shotID, _enemy.transform.position, _enemy.stat.attack, Vector2.zero);
    }

     public override void OnEnter()
    {   
    
        CanShift = false;
        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(dir.x, dir.z));
        _enemy.animationController.PlayAction(animationTag);
    }

    public override void OnExit()
    {   
        CanShift = true;
    }

}
