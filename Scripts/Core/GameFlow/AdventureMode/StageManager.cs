using UnityEngine;
using System.Collections.Generic;
using System;

public class StageManager
{
    //GameManagerに状態を知らせるためのイベント
    public event Action StageLoaded;
    public event Action StageCleared;
    public event Action AllStageFinished;

    private MapManager _mapManager;
    private WaveManager _waveManager;

    private int stageIndex;

    public Dictionary<int, StageData> stageDataDict;
    private StageData currentStage;
    
    //生成
    public StageManager()
    {
        stageDataDict = new Dictionary<int, StageData>();
        stageIndex = 1;
        LoadAllStageData("TestStage");
        SetCurrentStage(); 
    }

    //初期化
    public void Init(MapManager mapManager, WaveManager waveManager)
    {
        _mapManager = mapManager;
        _waveManager = waveManager;    
    }
    //JSONからステージデータロード
    private void LoadAllStageData(string currentWorld)
    {
         //Resourcesフォルダからjsonファイルを開く
        TextAsset jsonText = Resources.Load<TextAsset>("Data/StageData/" + currentWorld);

        if(jsonText != null)
        {
            StageDataList list = JsonUtility.FromJson<StageDataList>(jsonText.text);

            foreach(StageData data in list.stages){
                stageDataDict.Add(data.stageIndex, data);
            }
        }
        
        Debug.Log($"ステージデータのロード完了。読み込んだデータの数：{stageDataDict.Count}");
    }
    //現在のステージセット
    private void SetCurrentStage()
    {
        currentStage = stageDataDict[stageIndex];
    }

    //ステージをロード
    public void LoadStage()
    {
        //BGM設定
        BGMController.Instance.PlayByStageMusic(currentStage.stageMusic);
        //マップ初期化
        _mapManager.MapInit(currentStage.RoomID, currentStage.stageType);
        //敵・アイテムの初期配置
        _waveManager.WaveInit(currentStage.waveData);
        //デリゲートでステージ生成完了を知らせる
        StageLoaded?.Invoke();
        Debug.Log("ステージマネジャーがステージロード中");
    }
    //ステージが存在するかを確認
    public bool CheckHasNextStage(int stageIndex)
    {
        return stageIndex <= stageDataDict.Count;
    }

    //残りWaveがないならクリア判定
    public void CheckStageClear()
    {
        if(_waveManager.WaveUpdate())
        {
            return;
        }
        
        if(CheckHasNextStage(stageIndex+1))
        {
            StageCleared?.Invoke();
        }
        else{
            AllStageFinished?.Invoke();
        }
    }
    //ステージクリア時実行(指揮官クラスが)
    public void OnStageClear()
    {
        stageIndex++;
        SetCurrentStage();
        _mapManager.OpenGate(currentStage.stageType);
    }
    //ステージ開始時入口を閉じる
    public void OnStageStart()
    {
        _mapManager.BlockStart();
    }
}