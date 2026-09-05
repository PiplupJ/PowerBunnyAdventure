using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using TMPro;
using System;

//レベルシステムに関するUIを管理(例：レベルテキスト、経験値バー)
//LevelManagerの子供としてシーンに配置すること
public class LevelSystemUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image expBarImage; 
    [SerializeField] private float animationTime = 0.5f;
    private Coroutine _expFillCoroutine;

    //初期化
    public void Init()
    {
        UpdateLevelStatus(1);
        ClearExpBar();
    }

    //レベルアップの時、LevelManagerが実行
    public void OnLevelUp(int newLevel)
    {

        UpdateLevelStatus(newLevel);
        ClearExpBar();
    }

    private void UpdateLevelStatus(int newLevel)
    {
        levelText.text = "LV" + newLevel.ToString();
    }
    //経験値バーを更新。
    public void UpdateExpBar(float targetExpRatio, System.Action onComplete = null )
    {
        if(_expFillCoroutine!=null)
        {
            StopCoroutine(_expFillCoroutine);
        }
        _expFillCoroutine = StartCoroutine(ExpFillRoutine(targetExpRatio, onComplete));
    }
    //UpdateExpBarが呼び出す。滑らかに経験値バーを更新する
    private IEnumerator ExpFillRoutine(float targetExpRatio, System.Action onComplete)
    {
        //経験値ゲージが満タンを超えないように
        float target = Mathf.Min(targetExpRatio, 1.0f);

        float startExpRatio = expBarImage.fillAmount;
        float progress = 0.0f;
        
        while(progress < 1.0f){
            progress += Time.deltaTime/animationTime;
            expBarImage.fillAmount = Mathf.Lerp(startExpRatio, target, progress);

            yield return null;
        }
        
        expBarImage.fillAmount = target;
        
        onComplete?.Invoke();
    }

    private void ClearExpBar()
    {
        expBarImage.fillAmount = 0;
    }


}
