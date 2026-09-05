using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Random = UnityEngine.Random;
/*
Class AbilityCardRawData, public enum CardRarity { Normal, Rare, Epic }
はAbilityCard.csに定義されている
*/
public class AbilityCardData
{
    public int id;
    //カードの名前
    public string name;

    public Sprite cardSprite;
    //カードの説明
    public string description;
    //カードのレアリティ
    public CardRarity rarity;
}
public class AbilityCardLists
{
    public List<AbilityCardData> normalCards = new List<AbilityCardData>();
    public List<AbilityCardData> rareCards = new List<AbilityCardData>();
    public List<AbilityCardData> epicCards = new List<AbilityCardData>();
}

//操作用インターフェース
public interface ICardDrawControl
{
    void HandleCardSelection(Vector2 dirInput, bool selectPressed);
}

public class CardDrawManager : MonoBehaviour, ICardDrawControl
{
    private AbilityCardLists _abilityCardLists = new AbilityCardLists();

    private Dictionary<CardRarity, List<AbilityCardData>> rarityToListMap;

    //カード選択画面を管理
    [SerializeField] private CardDrawDisplayer _cardDrawDisplayer;
    
    //レアカード当たり確率。
    [SerializeField] float rareProbability = 0.3f;
    //エピックカード当たり確率
    [SerializeField] float epicProbability= 0.1f;
    //エピックカードに当たらなかった場合、当たるまで確率が上がっていく
    [SerializeField] float epicSoftPity = 0.01f;
    private int noEpicCount;

    //何枚のカードの中に一枚を選ぶか
    [SerializeField] private int cardPicks = 3;
    private List<AbilityCardData> pickedCards;
    private int currentPick;

    [SerializeField] private int slotImageSize = 7;
    
    private bool onSetUp = false;
    private float _inputTimer = 0f;
    private const float INPUT_DELAY = 0.2f;

    public event Action CardDrawFinished;

    private Player _player;
    
    //初期化
    public void Init()
    {
        noEpicCount = 0;

        LoadCardData();
        pickedCards = new List<AbilityCardData>();
        _cardDrawDisplayer.HideDisplay(); //カード選択画面は基本的に隠す
    }

    //プレイヤーをセッティング
    public void SetPlayer(Player player)
    {
        _player = player;
    }

    //JSONからカードデータロード
    private void LoadCardData()
    {
        //Resourcesフォルダからjsonファイルを開く
        TextAsset jsonText = Resources.Load<TextAsset>("Data/AbilityCards/CardDatas");
        
        //jsonファイルの中身をクラスのAbilityCardListsにする
        if(jsonText !=null){
            AbilityCardRawDataLists data = JsonUtility.FromJson<AbilityCardRawDataLists>(jsonText.text);
            
            ConvertAndAddCardData(data.normalCards, _abilityCardLists.normalCards);
            ConvertAndAddCardData(data.rareCards, _abilityCardLists.rareCards);
            ConvertAndAddCardData(data.epicCards, _abilityCardLists.epicCards);
        }

        rarityToListMap = new Dictionary<CardRarity, List<AbilityCardData>>()
        {
            { CardRarity.Normal, _abilityCardLists.normalCards },
            { CardRarity.Rare, _abilityCardLists.rareCards },
            { CardRarity.Epic, _abilityCardLists.epicCards }
        };
    }
    //jsonファイルデータを加工してゲーム用データを作成
    private void ConvertAndAddCardData(List<AbilityCardRawData> rawList, List<AbilityCardData> targetList)
    {
        if (rawList == null) return; 

        foreach (var rawCardData in rawList)
        {
            AbilityCardData newCardData = new AbilityCardData
            {
                id = rawCardData.id,
                name = rawCardData.name,
                cardSprite = Resources.Load<Sprite>(rawCardData.cardImagePath),
                description = rawCardData.description,
                rarity = rawCardData.rarity
            };
            targetList.Add(newCardData);
        }
    }
    //GameManagerが実行
    public void SetUpAndStartCardDraw()
    {
        CardRarity drawnRarity = CalculateRarity();
        StartCoroutine(CardDrawRoutine(drawnRarity));
    }

    public void SetUpAndStartCardDrawWithRarity(CardRarity selectedRarity)
    {
        StartCoroutine(CardDrawRoutine(selectedRarity));
    }

    //スロットマシンのような演出実行
    private IEnumerator CardDrawRoutine(CardRarity drawnRarity)
    {
        pickedCards.Clear();
        
        onSetUp = true;
        
        DrawRandomCardsFromList(drawnRarity);
        _cardDrawDisplayer.ShowDisplay();

        for(int i = 0; i < pickedCards.Count; i++)
        {
            //スロット用ランダムイメージ生成
            List<Sprite> slotSprites = CreateSlotImage(i, drawnRarity);
            //スロットにイメージ適用
            _cardDrawDisplayer.InitSlotImage(i, slotSprites);
        }

        Coroutine[] slotRoutines = new Coroutine[pickedCards.Count];
        for(int i = 0; i < pickedCards.Count; i++)
        {
            slotRoutines[i] = StartCoroutine(_cardDrawDisplayer.SlotSpinRoutine(i, pickedCards[i].name, pickedCards[i].description));
        }
        
        for(int i = 0; i < slotRoutines.Length; i++)
        {
            yield return slotRoutines[i];
        }
        
        onSetUp = false;
        currentPick = 0; 
        _cardDrawDisplayer.DisplayCurrentPick(currentPick);
        _cardDrawDisplayer.choiceDisplay.SetActive(true);
    }

    //今回引くカードのレアリティを求める
    private CardRarity CalculateRarity()
    {
        float currentEpicProbability = epicProbability + (epicSoftPity * noEpicCount);
        float roll = Random.value;

        if(roll<= currentEpicProbability)
        {
            noEpicCount = 0;
            return CardRarity.Epic;
        }
        //エピックに当たらなかった
        noEpicCount++;

        if(roll<= rareProbability + currentEpicProbability)
        {
            return CardRarity.Rare;
        }
        return CardRarity.Normal;
    }

    //求めたレアリティに対応するカードをリストから引く
    private void DrawRandomCardsFromList(CardRarity rarity)
    {
        List<AbilityCardData> targetList = rarityToListMap[rarity];

        if(targetList == null || targetList.Count == 0){
            Debug.LogError("引けるカードがありません。");
            return;
        }

        //ターゲットリストのカード数がイメージより少ない
        int picksThisDraw = Mathf.Min(cardPicks, targetList.Count);
        if(picksThisDraw < cardPicks)
        {
            Debug.LogWarning($"{rarity}のカードが{targetList.Count}種類しかありません。{cardPicks}分必要です");
        }
        
        for(int i = 0; i < picksThisDraw; i++)
        {
            int randomIndex = Random.Range(0, targetList.Count);
            AbilityCardData pickedCard = targetList[randomIndex];
            pickedCards.Add(pickedCard);
            targetList.RemoveAt(randomIndex);
        }
        //リストに戻る
        for(int i = 0; i < picksThisDraw; i++)
        {
            targetList.Add(pickedCards[i]);
        }
    }
    //スロットマシンに使うランダムイメージを生成
    private List<Sprite> CreateSlotImage(int slotID,CardRarity rarity)
    {
        List<AbilityCardData> targetList = rarityToListMap[rarity];

        if(targetList == null || targetList.Count == 0){
            Debug.LogError("引けるカードがありません。");
            return null;
        }
        List<Sprite> tempList = new List<Sprite>();

        Sprite firstAndLast = pickedCards[slotID].cardSprite;

        tempList.Add(firstAndLast);

        for(int i = 0; i < slotImageSize - 2; i++)
        {
            int randomIndex = Random.Range(0, targetList.Count);
            Sprite randomCardSprite = targetList[randomIndex].cardSprite;
            tempList.Add(randomCardSprite);
        }
        
        tempList.Add(firstAndLast);

        return tempList;
    }

    //PlayerController.csでCardDrawModeの時呼び出す。
    public void HandleCardSelection(Vector2 dirInput, bool selectPressed)
    {
        if(onSetUp) { return; }
        
        if(_inputTimer > 0){
            _inputTimer -= Time.deltaTime;
            return;
        }

        if(selectPressed){
            //選択したアイテムを生成、適用
            if(ObjectPool.Instance.TryGetObject<AbilityCard>(pickedCards[currentPick].id, out AbilityCard drawedCard))
            {
                drawedCard.ApplyEffect(_player);
            }
            else
            {
                Debug.LogError($"[CardDrawManager]カード{pickedCards[currentPick].id}の生成に失敗しました");
            }
            //UIを非表示処理
            _cardDrawDisplayer.HideDisplay();
            //イベント終了の通知
            CardDrawFinished?.Invoke();

            _inputTimer = INPUT_DELAY;
        }
        else if(dirInput.x!=0){
            UpdateCurrentPick(dirInput.x);
            _inputTimer = INPUT_DELAY;
        }

       
    }
    //入力に応じげ画面で表示するカードを変える
    private void UpdateCurrentPick(float xInput)
    {
        if(xInput > 0){
            currentPick = currentPick + 1 < pickedCards.Count ? currentPick + 1 : 0;
        }
        else{
            currentPick = currentPick - 1 >= 0 ? currentPick - 1 : pickedCards.Count - 1;
        }
        _cardDrawDisplayer.DisplayCurrentPick(currentPick);
    }
}
