/*
作成日6/12
チャレンジモードの管理者クラス
*/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class ChallengeModeManager : MonoBehaviour
{
    private Stack<GameState> _stateStack = new Stack<GameState>();
    public GameState currentGameState => _stateStack.Peek();

    //Drag and Drop
    public PlayerController playerController; //プレイヤ操作。ドラッグ＆ドロップ
    public LevelManager levelManager; //レベル情報やEXP状態を管理。ドラッグ＆ドロップ。
    public CardDrawManager cardDrawManager; //カード獲得イベントを管理。ドラッグ＆ドロップ
    public CameraController cameraController; //カメラ操作
    public PlayerHealthBarUI playerHealthUI;
    public ResultController resultController;

    //pure C# class
    private MapManager mapManager; //マップデータを管理
    private EntityManager entityManager; //敵、弾丸などのオブジェクトの生命周期を管理
    private CollisionManager collisionManager; //オブジェクト間の物理判定を管理
    private ChallengeWaveManager waveManager;

    private Player player;

    int waveIndex;
    [SerializeField] private int bossAppearInterval = 4;

    private void Awake()
    {
        //基本的にGameModeにするため、スタックにGameModeをプッシュ
        PushGameState(GameState.GameMode);

        //レベルシステムを初期化
        //レベルアップイベントが行ったらカード選択イベント開始
        //カード選択が終わったらゲームを再開
        levelManager.Init();
        levelManager.LevelUpEvent += StartCardDrawEvent;
        cardDrawManager.Init();
        cardDrawManager.CardDrawFinished += FinishCardDrawEvent;
        playerController.cardDrawControl = cardDrawManager;

        //ゲームシーンに必要なクラスを生成
        mapManager = new MapManager(); 
        entityManager = new EntityManager();
        collisionManager = new CollisionManager();
        waveManager = new ChallengeWaveManager();

        entityManager.AllEnemiesDead += CreateNextWave;

        waveIndex = 1;
    }

    private void Start()
    {
        PlayerDataManager.LoadPlayerDB();
        PlayerDataManager.SetCurrentPlayerID(1100001); //臨時
        //プレイヤを配置、初期化
        player = ObjectPool.Instance.GetObject<Player>(PlayerDataManager.currentPlayerID);
        player.Init(player.poolId, mapManager, entityManager, levelManager);
        playerController.SetPlayer(player);
        cameraController.SetTarget(player.transform);
        playerHealthUI.BindPlayer(player);
        player.PlayerIsDead += ProcessGameOver;

        levelManager.SetPlayer(player.transform);
        cardDrawManager.SetPlayer(player);

        entityManager.Init(player, mapManager);
        collisionManager.Init(entityManager);

        mapManager.MapInit(3200000, 0);

        waveManager.Init(entityManager, mapManager, bossAppearInterval);

        StageSetUp();
    }



    private void OnDestroy() 
    {    
        levelManager.LevelUpEvent -= StartCardDrawEvent;
        cardDrawManager.CardDrawFinished -= FinishCardDrawEvent;

        entityManager.AllEnemiesDead -= CreateNextWave;

        player.PlayerIsDead -= ProcessGameOver;
    }

    private void StageSetUp()
    {
        PushGameState(GameState.StandbyMode);

        playerController.IsAuto = true;
        player.transform.position = mapManager.startPos;
        float right = mapManager.mapTileSize * mapManager.mapWidth;
        float bottom = -(mapManager.mapTileSize * mapManager.mapHeight);
        cameraController.UpdateCameraBound(right, bottom);
        cameraController.SnapCameraPosition();
        float distToMove = mapManager.mapTileSize;
        StartCoroutine(playerController.Action.ExitFromPortal(distToMove, StartChallenge));
    }

    private void StartChallenge()
    {
        mapManager.BlockStart();
        playerController.IsAuto = false;
        PopGameState();
        waveManager.CreateWave(waveIndex);
    }

    private void Update()
    {
         float dt = Time.deltaTime;

        switch(currentGameState)
        {
            case GameState.GameMode :
                entityManager.HandleEntityState(dt);
                break;
            default :
                break;
        }
    }

    private void FixedUpdate()
    {
        if(currentGameState != GameState.GameMode) { return ;}
        //Pauseの場合、更新しない
        float dt = Time.fixedDeltaTime;
        entityManager.HandleEntityMovement(dt);
        collisionManager.HandleCollisions();

        entityManager.HandleExpItemMovement(dt);            
        collisionManager.HandleExpItemCollisions();

    }

    private void CreateNextWave()
    {
        this.waveIndex++;
        waveManager.CreateWave(waveIndex);
    }


     public void PushGameState(GameState newState)
    {
        _stateStack.Push(newState);
    }

    //臨時GameStateを終了
    public void PopGameState()
    {
        if(_stateStack.Count > 1)
        {
            _stateStack.Pop();
        }
    }

    public void StartCardDrawEvent()
    {
        PushGameState(GameState.CardDrawMode);
        playerController.PushControlMode(ControlMode.CardDrawMode);
        cardDrawManager.SetUpAndStartCardDraw();
    }

    //ボーナスステージなどで使う
    public void StartCardDrawEventWithRarity(CardRarity rarity)
    {
        PushGameState(GameState.CardDrawMode);
        playerController.PushControlMode(ControlMode.CardDrawMode);
        cardDrawManager.SetUpAndStartCardDrawWithRarity(rarity);
    }

    //カード選択イベント終了。状態を前の状態に戻す
    public void FinishCardDrawEvent()
    {
        levelManager.ExpCalculation(0);
        PopGameState();
        playerController.PopControlMode();
    }

    private void ProcessGameOver()
    {
        playerController.IsAuto = true;
        playerController.PushControlMode(ControlMode.PauseMode);
        PushGameState(GameState.PauseMode);
        resultController.Activate(GameResult.Defeat);
    }
}
