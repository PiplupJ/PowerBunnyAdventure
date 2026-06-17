using UnityEngine;
using System.Collections;

public class SummonBehaviour : EnemyBehaviour
{
    [SerializeField] private int _summonID;
    [SerializeField] private float castFrame;
    [SerializeField] private float summonFrame;
    [SerializeField] private float summonInterval = 2.0f;
    [SerializeField] private float summonDistance = 2.5f;
    [SerializeField] private string animationTag;

    private enum SummonPhase { WindUp, Cast, Summon, Done }
    private SummonPhase _phase;

    private bool CanSummon;
    private Vector3 summonPos;

    public override void Init(Enemy enemy)
    {
        StopAllCoroutines();
        _phase = SummonPhase.WindUp;
        CanSummon = true;
        base.Init(enemy);
    }

    public override bool CanExecute()
    {
        return CanSummon;
    }

    public override void Execute(float fixedDeltaTime)
    {
        float t = _enemy.animationController.GetNormalizedTime("Attack");

        switch(_phase)
        {
            case SummonPhase.WindUp :
                if(t >= castFrame)
                {
                    _phase = SummonPhase.Cast;
                }
                break;
            case SummonPhase.Cast :
                if(t >= summonFrame)
                {
                    PoolableObject summonEffect = ObjectPool.Instance.GetObject<PoolableObject>(IDRegistry.SUMMON_EFFECT);
                    summonEffect.transform.position = summonPos;
                    _phase = SummonPhase.Summon;
                }
                break;
            case SummonPhase.Summon :
                if(t >= 1.0f)
                {
                    _enemy._enemyUpdater.CreateEnemyAtPosition(_summonID, summonPos);
                    _phase = SummonPhase.Done;
                }
                break;
            case SummonPhase.Done :
                StartCoroutine(SummonCooldownRoutine(summonInterval));
                OnExit();
                break;
        }
    }

    public override void OnEnter()
    {   
        CanShift = false;
        _phase = SummonPhase.WindUp;
        summonPos = _enemy.transform.position + _enemy.transform.forward * summonDistance;

        _enemy.animationController.PlayAction(animationTag);
    }

    public override void OnExit()
    {   
        CanShift = true;
    }

    protected IEnumerator SummonCooldownRoutine(float coolTime)
    {
        CanSummon = false;

        yield return new WaitForSeconds(coolTime);
        CanSummon = true;
    }
}
