using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    protected readonly int BlendMotionHash = Animator.StringToHash("BlendMotion");
    protected readonly int SpeedHash = Animator.StringToHash("Speed");

    protected readonly int DeathHash = Animator.StringToHash("Death");

    protected const float CrossFadeDuration = 0.1f;
    protected const float AnimatorDampTime = 0.1f;
    protected const float transitionDuration = 0.1f;

    protected void TryBindBlendMotion<T>() where T : EnemyBehaviour
    {
        T b = GetComponent<T>();
        if(b == null) { return; }
        b.OnBehaviourEnter += () => SwitchToBlendMotion();
        b.OnBehaviourExit += () => UpdateBlendMotion(0.0f);
    }

    protected void SwitchToBlendMotion()
    {
        int currentHash = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        
        if(currentHash!=BlendMotionHash)
        {
            _animator.CrossFadeInFixedTime(BlendMotionHash, CrossFadeDuration);
        }
    }

    public void UpdateBlendMotion(float speed)
    {
        _animator.SetFloat(SpeedHash, speed, AnimatorDampTime, Time.deltaTime);
    }

    public void BindDeathEvent(Enemy enemy)
    {
        enemy.OnDeath += () => _animator.CrossFadeInFixedTime(DeathHash, transitionDuration);    
    }

    public void PlayAction(string tag)
    {
        int hash = Animator.StringToHash(tag);
        _animator.CrossFadeInFixedTime(hash, transitionDuration);
    }

    protected void TryBindAction<T>(int hash) where T : EnemyBehaviour
    {
        T b = GetComponent<T>();
        if(b == null) 
        {
            return; 
        }
        b.OnBehaviourEnter += () => _animator.CrossFadeInFixedTime(hash, transitionDuration);
    }

    public float GetNormalizedTime(string tag)
    {
        AnimatorStateInfo currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo    = _animator.GetNextAnimatorStateInfo(0);

        if (_animator.IsInTransition(0) && nextInfo.IsTag(tag))
            return nextInfo.normalizedTime;

        if (!_animator.IsInTransition(0) && currentInfo.IsTag(tag))
            return currentInfo.normalizedTime;

        return 0f;
    }

}
