using UnityEngine;
using System;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public int Priority;

    public bool CanShift = true;

    protected Enemy _enemy;

    public event Action OnBehaviourEnter;
    public event Action OnBehaviourExit;

    public virtual void Init(Enemy enemy)
    {
        _enemy = enemy;
    }

    public abstract bool CanExecute();
    public abstract void Execute(float fixedDeltaTime);

    public virtual void OnEnter() => OnBehaviourEnter?.Invoke();
    public virtual void OnExit() => OnBehaviourExit?.Invoke();

    public void SetPriority(int num)
    {
        Priority = num;
    }
}
