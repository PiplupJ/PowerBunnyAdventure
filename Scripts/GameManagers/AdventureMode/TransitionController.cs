using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private Image maskImage;
    [SerializeField] private float transitionSpeed = 2.0f;

    public void StartTransition(Action onScreenCovered)
    {
        StartCoroutine(TransitionRoutine(onScreenCovered));
    }

    private IEnumerator TransitionRoutine(Action onScreenCovered)
    {
        yield return StartCoroutine(ScreenFade(0.0f, 1.0f));

        onScreenCovered?.Invoke();

        yield return StartCoroutine(ScreenFade(1.0f,0.0f));
    }

    private IEnumerator ScreenFade(float from, float to)
    {
        float elapsedTime = 0.0f;
        Color color = maskImage.color;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            color.a = Mathf.Lerp(from, to, elapsedTime);
            maskImage.color = color;
            yield return null;
        }

        color.a = to;
        maskImage.color = color;

    }

}
