using UnityEngine;
using System.Collections.Generic;

public class MovingEnemyAnimatorController : EnemyAnimationController
{
    private static readonly int _hashWalk = Animator.StringToHash("Walk");
    //private static readonly int _hashMelee = Animator.StringToHash("Base Layer.Headbutt");
    private static readonly int _hashIdle = Animator.StringToHash("Idle");
    //private static readonly int _hashShoot = Animator.StringToHash("Shot");

    private void Awake()
    {
        TryBindBlendMotion<IdleBehaviour>();
        TryBindBlendMotion<ChaseBehaviour>();
        
        //TryBindAction<MeleeAttackBehaviour>(_hashMelee);
        //TryBindAction<RangedAttackBehaviour>(_hashShoot);
    }

}
