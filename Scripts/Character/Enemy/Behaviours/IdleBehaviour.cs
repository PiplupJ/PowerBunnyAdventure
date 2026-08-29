using UnityEngine;
using System;

public class IdleBehaviour : EnemyBehaviour
{

    public override bool CanExecute()
    {
        return true;
    }

    public override void Execute(float fixedDeltaTime)
    {
        _enemy.animationController.UpdateBlendMotion(0.0f);
    }
}
