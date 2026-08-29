using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    protected readonly int BlendMotionHash = Animator.StringToHash("BlendMotion");
    protected readonly int SpeedHash = Animator.StringToHash("Speed");

    protected const float CrossFadeDuration = 0.1f;
    protected const float AnimatorDampTime = 0.1f;
    protected const float transitionDuration = 0.1f;

    private static readonly int _hashAttack = Animator.StringToHash("Base Layer.Attack");
    private static readonly int _hashDeath = Animator.StringToHash("Base Layer.Death");

    public void BindToMotion(Player player)
    {
        player.IsAttacking += TryAttack;
        player.PlayerIsDead += TryDeath;
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

    protected void TryAttack()
    {
        AnimatorStateInfo current = _animator.GetCurrentAnimatorStateInfo(0);
        if (current.IsTag("Attack")) return;
        _animator.CrossFadeInFixedTime(_hashAttack, transitionDuration);
    }

    protected void TryDeath()
    {
        _animator.CrossFadeInFixedTime(_hashDeath, transitionDuration);
    }

    public void StopBlendMotion()
    {
        _animator.SetFloat(SpeedHash, 0f, 0f, Time.deltaTime);  
    }

    public void PlayAction(string tag)
    {
        int hash = Animator.StringToHash(tag);
        _animator.CrossFadeInFixedTime(hash, transitionDuration);
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
