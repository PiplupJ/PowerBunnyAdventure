using UnityEngine;
using System.Collections;
//爆発による影響
//エフェクトがある時間中継続
public class ExplosionEffectShot : Shot
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float lifeSpan = 1.0f;

    private void OnEnable()
    {
        audioSource.PlayOneShot(sfx);
        particle.Play();
        StartCoroutine(ReturnRoutine());
    }

    private void OnDisable() 
    {
        particle.Stop();
        audioSource.Stop();    
    }

    private IEnumerator ReturnRoutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        OnDie();
    }

    public override void HandleShotAction(float deltaTime)
    {
    }


    public override void OnAttack()
    {
    }
}
