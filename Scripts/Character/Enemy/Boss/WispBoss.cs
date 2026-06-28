using UnityEngine;

public class WispBoss : Enemy
{
    [Header("Phase 2 Behaviours")]
    [SerializeField] private EnemyBehaviour enemySummon;
    [SerializeField] private EnemyBehaviour kiteMovement;

    [Header("Final Phase Behaviour")]
    [SerializeField] private EnemyBehaviour meteorSummon;

    [SerializeField] private float hpRatioP2 = 0.6f;
    [SerializeField] private float hpRatioP3 = 0.3f;

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private BossPhase _phase;

    private void OnEnable() {
        this._phase = BossPhase.Phase1;    
    }

    public override void OnHit(int damage, bool wasCritical)
    {
        base.OnHit(damage, wasCritical);

        //体力数値が一定以上下がったら次のフェイズに入る
        if(GetHpRatio()<=hpRatioP2 && _phase < BossPhase.Phase2)
        {
            EnterPhase2();
        }

        if(GetHpRatio()<=hpRatioP3 && _phase < BossPhase.Phase3)
        {
            EnterPhase3();
        }
        
    }

    //フェイズ2
    private void EnterPhase2()
    {
        enemySummon.SetPriority(150);
        kiteMovement.SetPriority(80);
        
        _phase = BossPhase.Phase2;
        //行動リスト再整列
        _behaviours.Sort((a,b)=> b.Priority.CompareTo(a.Priority));
    }
    //フェイズ3
    private void EnterPhase3()
    {
        meteorSummon.SetPriority(200);
        _phase = BossPhase.Phase3;
        //行動リスト再整列
        _behaviours.Sort((a,b)=> b.Priority.CompareTo(a.Priority));
    }

}
