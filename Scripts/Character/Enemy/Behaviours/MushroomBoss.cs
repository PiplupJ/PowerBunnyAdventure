using UnityEngine;

public class MushroomBoss : Enemy
{
    private enum BossPhase { Phase1, Phase2 }
    private BossPhase _phase;

    [Header("Phase 2 Behaviours")]
    [SerializeField] private EnemyBehaviour enemySummon;
    [SerializeField] private EnemyBehaviour doubleSplitShot;

    private void OnEnable() {
        this._phase = BossPhase.Phase1;    
    }

    public override void OnHit(int damage, bool wasCritical)
    {
        base.OnHit(damage, wasCritical);

        if(GetHpRatio()<0.5f && _phase < BossPhase.Phase2)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        _phase = BossPhase.Phase2;

        enemySummon.SetPriority(200);
        doubleSplitShot.SetPriority(150);

        _behaviours.Sort((a,b)=> b.Priority.CompareTo(a.Priority));
    }
}
