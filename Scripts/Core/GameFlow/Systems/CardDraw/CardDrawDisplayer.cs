using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class CardDisplayWindow
{
    public GameObject parent;
    public TextMeshProUGUI name;
    public RectTransform reel;
    //スロットマシンのような演出のため配列
    //7個に構成すること。0,6は当たりイメージ。1~5はランダム
    //ImageオブジェクトのSpriteを指定すること。
    public List<Image> cardImages = new List<Image>();
    public TextMeshProUGUI description;
}
public class CardDrawDisplayer : MonoBehaviour
{
    public float animationDuration = 2.0f;
    public float animationInterval = 0.1f;
    public float slotSpeed = 1200.0f;
    public int slotSpinCount = 5;
    
    public CardDisplayWindow [] cardDisplays = new CardDisplayWindow[3];

    public GameObject choiceDisplay;
    //予備用
    public GameObject slotSprite;

    public void ShowDisplay()
    {
        ClearCardDisplayWindow();
        for(int i = 0; i < cardDisplays.Length; i++)
        {
            cardDisplays[i].parent.SetActive(true);
        }
    }

    public void InitSlotImage(int slotID, List<Sprite>slotSprites)
    {
        for(int i = 0; i < slotSprites.Count; i++)
        {
            cardDisplays[slotID].cardImages[i].sprite = slotSprites[i];
            Debug.Log("スロットイメージ作業完了"+slotID);
        }
    }

    public IEnumerator SlotSpinRoutine(int slotID, string resultName, string resultDescription)
    {
        CardDisplayWindow currentSlot = cardDisplays[slotID];
        int spinCount = slotID + slotSpinCount; //スロット１、スロット２、スロット３順に回転が終わる
        Vector2 startPos = currentSlot.reel.anchoredPosition;
        //イメージの座標をリセットする地点を求める
        float slotHeight = currentSlot.reel.rect.height;
        float targetY = -(currentSlot.cardImages.Count - 1) * slotHeight;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        int currentSpinCount = 0; 

        float speed = slotSpeed;

        while(currentSpinCount < spinCount)
        {
            currentSlot.reel.anchoredPosition -= new Vector2(0, speed* Time.deltaTime);
            //現在の回転数を更新
            if(currentSlot.reel.anchoredPosition.y <= targetY)
            {
                currentSlot.reel.anchoredPosition = startPos;
                currentSpinCount++;
                Debug.Log("スロット回転数:"+currentSpinCount);
            }
            yield return null;    
        }

        //ここからはスロットが徐々に動く
        float elapsedTime = 0f;
        float duration = 0.5f;
    
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = 1.0f - (1.0f - t) * (1.0f - t);

            currentSlot.reel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }
        //スロットの結果を見せる
        currentSlot.reel.anchoredPosition = startPos;
        currentSlot.name.text = resultName;
        currentSlot.description.text = resultDescription;
    }

    public void HideDisplay()
    {
        for(int i = 0; i < cardDisplays.Length; i++)
        {
            cardDisplays[i].parent.SetActive(false);
        }
        choiceDisplay.SetActive(false);
    }

    //現在のディスプレイデータを初期化
    private void ClearCardDisplayWindow()
    {
        foreach(var cardDisplay in cardDisplays)
        {
            cardDisplay.name.text = "";
            for(int i = 0; i < cardDisplay.cardImages.Count; i++)
            {
                cardDisplay.cardImages[i].sprite = null;
            }
            cardDisplay.description.text = "";
        }
    }

    //選択したカードを表示するオブジェクトを移動する
    public void DisplayCurrentPick(int currentPick)
    {
        choiceDisplay.transform.position = cardDisplays[currentPick].parent.transform.position;
    }
}
