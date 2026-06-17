using UnityEngine;
using System.Collections;

public class HitEffect : PoolableObject
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float effectDuration;

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
        yield return new WaitForSeconds(effectDuration);
        ReturnToPool();
    }
}
