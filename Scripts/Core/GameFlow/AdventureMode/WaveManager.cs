using UnityEngine;
using System.Collections.Generic;
using System;

public class WaveManager
{
    private IEnemySpawner _enemySpawner;

    private int waveIndex; //現在のウェーブインデックス
    private List<WaveData> _waveData; //現在のウェーブリスト

    //初期化。GameManagerが実行
    public WaveManager()
    {
        waveIndex = 0;
        _waveData = new List<WaveData>();
    }
    //変数初期化。GameManagerが実行
    public void Init(IEnemySpawner newEnemySpawner)
    {
       _enemySpawner = newEnemySpawner;
    }

    //ウェーブを初期化。StageManagerが実行
    public void WaveInit(List<WaveData> waveData)
    {
        //ウェーブがない場合。ボーナスステージなど
        if(waveData.Count==0)
        {
            _waveData = new List<WaveData>(); 
            waveIndex = 0;
            return;
        }
        _waveData = waveData;
        waveIndex = 0;
        WaveCreate();
    }

    //ウェーブ通りに敵を配置
    public void WaveCreate()
    {
        var currentWaveItems = _waveData[waveIndex].items;

        if(currentWaveItems != null && currentWaveItems.Count > 0)
        {
            foreach(var itemData in currentWaveItems)
            {
                _enemySpawner.CreateDropItem(itemData.id, itemData.x, itemData.z);
            }
        }

        var currentWaveEnemies = _waveData[waveIndex].enemies;

        if (currentWaveEnemies == null || currentWaveEnemies.Count == 0)
        {
            Debug.Log("It was Bonus Stage. No enemy!");
            _enemySpawner.CheckAllEnemiesDead();
            return;
        }

        foreach (var enemyData in currentWaveEnemies)
        {
            _enemySpawner.CreateEnemy(enemyData.id, enemyData.x, enemyData.z);
        }
        Debug.Log("ウェーブロード完了");
    }
    //次のウェーブを生成。ないならFalseを返却
    public bool WaveUpdate()
    {
        waveIndex++;
        if(waveIndex < _waveData.Count){
            WaveCreate();
            return true;
        }
        return false;
    }
}
