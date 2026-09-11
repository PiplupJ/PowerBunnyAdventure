using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//ゲームの状態
//Standby 待機状態。
//Game ゲーム進行中
//Pause　一時停止
//CardDrawMode レベルアップによってカードを引くイベント
public enum GameState
{
    StandbyMode, GameMode, PauseMode, CardDrawMode
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //GameStateはStackで管理。
    //基本的にGameMode
    private Stack<GameState> _stateStack = new Stack<GameState>();
    public GameState currentGameState => _stateStack.Peek();

    
    //現在のステージをクリアしたか。ステージ進行中なのに次のステージがロードされないため使います。
    private bool stageClearFlag;

    //GameManagerの子供としてシーンに配置すること
    public PlayerController playerController; //プレイヤ操作。ドラッグ＆ドロップ
    public LevelManager levelManager; //レベル情報やEXP状態を管理。ドラッグ＆ドロップ。
    public CardDrawManager cardDrawManager; //カード獲得イベントを管理。ドラッグ＆ドロップ
    public CameraController cameraController; //カメラ操作
    public TransitionController transition; //演出用FadeIn,FadeOut操作
    public PlayerHealthBarUI playerHealthUI;
    public ResultController resultController;

    //純粋C#クラスでGameManagerが生成
    private MapManager mapManager; //マップデータを管理
    private EntityManager entityManager; //敵、弾丸などのオブジェクトの生命周期を管理
    private CollisionManager collisionManager; //オブジェクト間の物理判定を管理
    private WaveManager waveManager; //ステージに登場する敵のウェーブを管理
    private StageManager stageManager; //ステージをロード
    
    //プレイヤ。GameManagerがシーンに配置
    [SerializeField] private int playerID = 1100001;
    public Player player; //プレイヤ。

    //初期化１:変数の初期化、クラスの生成、クラスのイベントをサブスクリプト
    private void Awake()
    {
        //シングルトーンの重複を防ぐ
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
 
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
        waveManager = new WaveManager();
        stageManager = new StageManager();
        collisionManager = new CollisionManager();

        stageManager.StageLoaded += StageStart;
        stageManager.StageCleared += StageFinish;
        stageManager.AllStageFinished += ProcessGameClear;

    }

    //破壊する時、サブスクリプションを解除
    private void OnDestroy() 
    {    
        levelManager.LevelUpEvent -= StartCardDrawEvent;
        cardDrawManager.CardDrawFinished -= FinishCardDrawEvent;

        stageManager.StageLoaded -= StageStart;
        stageManager.StageCleared -= StageFinish;
        stageManager.AllStageFinished -= ProcessGameClear;
        //StageManagerのイベントサブスクリプション削除
        entityManager.AllEnemiesDead -= stageManager.CheckStageClear;

        player.PlayerIsDead -= ProcessGameOver;
    }
    
    //初期化2:ゲームシーンをセットアップ、ステージ１生成
    private void Start()
    {
        PlayerDataManager.LoadPlayerDB();
        PlayerDataManager.SetCurrentPlayerID(playerID); 
        //プレイヤを配置、初期化
        if(!ObjectPool.Instance.TryGetObject<Player>(PlayerDataManager.currentPlayerID, out player))
        {
            Debug.LogError($"[GameManager]プレイヤー{PlayerDataManager.currentPlayerID}の生成に失敗しました");
            return;
        }
        player.Init(player.poolId, mapManager, entityManager, levelManager);
        playerController.SetPlayer(player);
        cameraController.SetTarget(player.transform);
        playerHealthUI.BindPlayer(player);
        player.PlayerIsDead += ProcessGameOver;

        levelManager.SetPlayer(player.transform);
        cardDrawManager.SetPlayer(player);

        entityManager.Init(player, mapManager);
        collisionManager.Init(entityManager);
        waveManager.Init(entityManager);
        stageManager.Init(mapManager, waveManager);
        entityManager.AllEnemiesDead += stageManager.CheckStageClear;
      
        stageClearFlag = false;
        //ステージを生成
        StageLoad();
    }
    
    //更新
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
    
    //物理的な移動や衝突関連更新
    private void FixedUpdate()
    {
        if(currentGameState != GameState.GameMode) { return ;}
        //Pauseの場合、更新しない
        float dt = Time.fixedDeltaTime;
        entityManager.HandleEntityMovement(dt);
        collisionManager.HandleCollisions();
        //ステージクリア時
        if(stageClearFlag)
        {
            entityManager.HandleExpItemMovement(dt);
            collisionManager.HandleExpItemCollisions();
        }
    }

    //ステージをロード
    private void StageLoad()
    {
        PushGameState(GameState.StandbyMode);
        stageManager.LoadStage();
        Debug.Log("ステージロード中");
    }
    //ステージ開始演出実行
    private void StageStart()
    {
        //プレイヤを自動移動
        playerController.IsAuto = true;
        player.transform.position = mapManager.startPos;
        float right = mapManager.mapTileSize * mapManager.mapWidth;
        float bottom = -(mapManager.mapTileSize * mapManager.mapHeight);
        cameraController.UpdateCameraBound(right, bottom);
        cameraController.SnapCameraPosition();
        mapManager.touchedFinish += ToNextStage;

        float distToMove = mapManager.mapTileSize;
        StartCoroutine(playerController.Action.ExitFromPortal(distToMove, StageStartAction));
        //マップの出発地点に戻りません。
    }
    //ステージ開始演出終了後、ゲームモードに入る
    private void StageStartAction()
    {
        stageManager.OnStageStart();
        playerController.IsAuto = false;
        PopGameState();
    }

    //StageManagerがステージが終わったと知らせたら実行
    private void StageFinish()
    {
        stageClearFlag = true;
        //マップのゲートを開く
        stageManager.OnStageClear();
    }
    //次のステージへ
    private void ToNextStage()
    {
        //残り作業があればreturn
        if (entityManager.ExpItems.Count > 0) return;
        if (currentGameState != GameState.GameMode) return;
        
        mapManager.touchedFinish -= ToNextStage;

        Vector3 portalPos = mapManager.goalPos;
        Debug.Log($"ポータルの座標x:{portalPos.x}y:{portalPos.y}z:{portalPos.z}");
        playerController.IsAuto = true;
        StartCoroutine(playerController.Action.EnterToPortal(portalPos, OnStageTransition));
    }
    //ステージ転換時、フェードイン・フェードアウト
    public void OnStageTransition()
    {
        transition.StartTransition(LoadNextStage);
    }
    //MapManagerのポータル発動イベントで呼び出す
    public void LoadNextStage()
    {
        //現在のステージをクリアした状態なら次のステージを呼び出す
        if(stageClearFlag==false) { return; }
        stageClearFlag = false;
        entityManager.ClearAllEntities();
        StageLoad();
    }
    
    //基本的にGameStateはGameModeが、臨時的に他のGameStateに変更する場合呼び出す
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
    //アイテム獲得イベント開始
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

    //クリアシーンに移動
    private void ProcessGameClear()
    {
        playerController.IsAuto = true;
        playerController.PushControlMode(ControlMode.PauseMode);
        PushGameState(GameState.PauseMode);
        resultController.Activate(GameResult.Victory);
    }
    //ゲームオーバー
    private void ProcessGameOver()
    {
        playerController.IsAuto = true;
        playerController.PushControlMode(ControlMode.PauseMode);
        PushGameState(GameState.PauseMode);
        resultController.Activate(GameResult.Defeat);
    }
}
