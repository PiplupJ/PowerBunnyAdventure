using UnityEngine;
using System;
using System.Collections;

public class RangedAttackBehaviour : EnemyBehaviour
{
    //攻撃状態に入る範囲
    [SerializeField] private float _attackRange = -1f;
    private float AttackRange => _attackRange > 0f ? _attackRange : _enemy.stat.attackDist;

    [SerializeField] private int _shotID; //生成する弾のID
    [SerializeField] private float shootingFrame; //モーション中攻撃を行うフレーム
    [SerializeField] private float recoverFrame; //攻撃を終了するフレーム
    [SerializeField] private float attackInterval = 1.0f; //クールダウン時間
    [SerializeField] private string animationTag; //モーションのタグ

    [SerializeField] private float firePointOffset = 1.0f; //弾の発射点（自分の前からどのぐらいか）

    //攻撃の段階
    private enum AttackPhase { WindUp, Active, Recover, Done }

    private AttackPhase _phase;

    private bool CanAttack;

    //弾発射向き
    protected Vector3 shotDir;

    //初期化
    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        CanAttack = true;
        _phase    = AttackPhase.WindUp;
        
        base.Init(enemy);
    }
    
    //実行できるかを返却
    public override bool CanExecute()
    {
        return CanAttack && DistanceHelper.IsInRange(_enemy.transform.position, _enemy.target.position, AttackRange);
    }

    //実行
    public override void Execute(float fixedDeltaTime)
    {
        //モーションの状態を確認
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case AttackPhase.WindUp :
                if(t >= shootingFrame)
                {
                    //攻撃を実行し、弾を発射
                    SpawnShot();
                    _phase = AttackPhase.Active;
                }
                break;
            case AttackPhase.Active :
                if(t >= recoverFrame)
                {
                    //攻撃終了
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
                //クールダウン開始
                StartCoroutine(AttackCooldownRoutine(attackInterval/_enemy.stat.attackSpeed));
                //終了。他のステートに転換可能
                CanShift = true;
                break;
        }
    }
    //弾生成
    public void SpawnShot()
    {
        _enemy._enemyUpdater.CreateEnemyShot(_shotID, GetFirePoint(), _enemy.stat.attack, shotDir);
    }
    //発射点を返却
    public Vector3 GetFirePoint()
    {
        return _enemy.transform.position + 
               _enemy.transform.forward * firePointOffset;
    }

    public override void OnEnter()
    {   
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
