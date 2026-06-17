using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MeteorAttackBehaviour : EnemyBehaviour
{
    [SerializeField] private int _shotID;
    [SerializeField] private float totalAttackTime = 5.0f;
    [SerializeField] private string windUpMotion;
    [SerializeField] private string activeMotion;
    [SerializeField] private string castingMotion;
    [SerializeField] private string recoverMotion;
    [SerializeField] private float attackInterval = 5.0f;
    float attackTimeCount;
    [SerializeField] private float meteorInterval = 0.5f;
    [SerializeField] private float minSpawnRadius = 1.0f;
    [SerializeField] private float maxSpawnRadius = 5.0f;
    bool canCastMeteor;

    [SerializeField] private int meteorCount = 4;

    private enum AttackPhase { WindUp, Active, Casting, Recover, Done }
    private AttackPhase _phase;

    bool CanAttack;

     public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        _phase = AttackPhase.WindUp;
        CanAttack = true;
        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanAttack;
    }

    public override void OnEnter()
    {
        CanShift = false;
        attackTimeCount = totalAttackTime;
        _phase = AttackPhase.WindUp;
        Vector3 direction = (_enemy.target.position - _enemy.transform.position).normalized;
        _enemy.RotateToTarget(new Vector2(direction.x, direction.z));
        _enemy.animationController.PlayAction(windUpMotion);
        canCastMeteor = true;
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case AttackPhase.WindUp :
                if(t>=1){
                    _phase = AttackPhase.Active;
                    _enemy.animationController.PlayAction(activeMotion);
                }
                break;
            case AttackPhase.Active :
                if(t>=1){
                    _phase = AttackPhase.Casting;
                    _enemy.animationController.PlayAction(castingMotion);
                }
                break;
            case AttackPhase.Casting :
                if(canCastMeteor){
                    StartCoroutine(MeteorCreateRoutine(meteorInterval));
                }
            
                attackTimeCount-=fixedDeltaTime;

                if(attackTimeCount<=0){
                    _phase = AttackPhase.Recover;
                    _enemy.animationController.PlayAction(recoverMotion);
                }
                break;
            case AttackPhase.Recover :
                if(t>=1){
                    _phase = AttackPhase.Done;
                }
                break;
            case AttackPhase.Done :
                StartCoroutine(AttackCooldownRoutine(attackInterval/_enemy.stat.attackSpeed));
                CanShift = true;
                break;
        }
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

    protected IEnumerator MeteorCreateRoutine(float coolTime)
    {
        canCastMeteor = false;
        CreateMeteor();

        yield return new WaitForSeconds(coolTime);

        canCastMeteor = true;
    }

    void CreateMeteor()
    {
        for(int i = 0; i < meteorCount; i++){
            Vector3 spawnPos = GetMeteorPos();
             _enemy._enemyUpdater.CreateEnemyShot(_shotID, spawnPos, _enemy.stat.attack, Vector2.zero);
        }
        
    }

    Vector3 GetMeteorPos()
    {
        Vector3 targetPos = _enemy.target.position;

        for(int i = 0; i < 6; i++){
            float angle = Random.value * Mathf.PI *2f;
            float r = Mathf.Sqrt(
                Random.value * (maxSpawnRadius* maxSpawnRadius - minSpawnRadius * minSpawnRadius)
                + minSpawnRadius* minSpawnRadius
            );

            Vector3 modifiedPos = targetPos + new Vector3(Mathf.Cos(angle)*r, 0f, Mathf.Sin(angle)*r);

            if(!_enemy._mapCollision.MapWallHitCheck(modifiedPos, 1f)){
                return modifiedPos;
            }
        }
        return targetPos;
    }
}
