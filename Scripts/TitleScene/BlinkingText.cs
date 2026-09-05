using UnityEngine;
using TMPro;
using System.Collections;

//点滅するテキストを制御するクラス
public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mText;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float blinkInterval = 0.6f;

    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        blinkCoroutine = StartCoroutine(ExecuteAfterDelay());
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        mText.alpha = 1f;

        // 点滅をループする
        blinkCoroutine = StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return StartCoroutine(FadeTo(minAlpha, blinkInterval));
            yield return StartCoroutine(FadeTo(1f, blinkInterval));
        }
    }

    //InOutSine イージングによる点滅
    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = mText.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EaseInOutSine(t);
            mText.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedT);
            yield return null;
        }

        mText.alpha = targetAlpha;
    }

    //DOTweenの Ease.InOutSine
    private float EaseInOutSine(float t)
    {
        return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    //テキストを隠す
    public void Hide()
    {
        mText.gameObject.SetActive(false);
        this.enabled = false;
    }
}