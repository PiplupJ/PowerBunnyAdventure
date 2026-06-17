using UnityEngine;

public class EventCharacterAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    protected const float CrossFadeDuration = 0.1f;
    protected const float AnimatorDampTime = 0.1f;
    protected const float transitionDuration = 0.1f;

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
