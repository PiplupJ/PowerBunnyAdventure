using UnityEngine;
using System;

public abstract class EnemyBehaviour : MonoBehaviour
{
    //優先度
    public int Priority;

    //他のステートに変更できるか
    public bool CanShift = true;
    //行動の主体
    protected Enemy _enemy;

    public event Action OnBehaviourEnter;
    public event Action OnBehaviourExit;

    //初期化
    public virtual void Init(Enemy enemy)
    {
        _enemy = enemy;
    }

    //条件を満たしているか
    public abstract bool CanExecute();
    //実行
    public abstract void Execute(float fixedDeltaTime);

    //ステート開始
    public virtual void OnEnter() => OnBehaviourEnter?.Invoke();
    //ステート終了
    public virtual void OnExit() => OnBehaviourExit?.Invoke();

    //優先度再設定
    public void SetPriority(int num)
    {
        Priority = num;
    }
}
