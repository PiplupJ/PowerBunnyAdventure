using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EnemyIdList
{
    public List<int> ids;
}
[System.Serializable]
public class EnemyPool
{
    public List<EnemyIdList> enemies;
    public List<EnemyIdList> boss;
}

public class ChallengeWaveManager 
{
    private IEnemySpawner _enemySpawner;
    private MapManager map;

    EnemyPool enemyPool;
    private int bossAppearInterval;

    private GridContext grid;

    private List<PatternEntry> spawnPatterns;

    public ChallengeWaveManager()
    {
        LoadEnemyPool();
    }

    public void Init(IEnemySpawner newEnemySpawner, MapManager mapManager, int newBossAppearInterval)
    {
        _enemySpawner = newEnemySpawner;
        map = mapManager;
        bossAppearInterval = newBossAppearInterval;

        grid = new GridContext{ width = map.mapWidth, height = map.mapHeight};
        BuildPatterns();
    }

    
    private void LoadEnemyPool()
    {
        //Resourcesフォルダからjsonファイルを開く
        TextAsset jsonText = Resources.Load<TextAsset>("Data/EnemyPool");

        if (jsonText != null)
        {
            // JsonUtilityを使い、テキストをExpDataオブジェクトに変更
            enemyPool = JsonUtility.FromJson<EnemyPool>(jsonText.text);
        }
        else
        {
            Debug.LogError("EnemyPool.jsonファイルはありません");
        }
    }
    //現在のマップグリッドをもとにパターン生成
    private void BuildPatterns()
    {
        spawnPatterns = new List<PatternEntry>
        {
            new(new ClusterPattern(), 0, 1f),
            new(new CornersPattern(), 0, 1f),
            new(new EdgeLinePattern(), 1, 1f),
            new(new CrossPattern(), 2, 1f)
        };
    }

    public void CreateWave(int waveCount)
    {

        int keyValue = (waveCount-1)/bossAppearInterval;

        if(waveCount%bossAppearInterval==0){
            GenerateBoss(keyValue);
        }
        else{
            GenerateEnemies(keyValue);
        }
    }
    //ボスの場合、マップ中央
    private void GenerateBoss(int value)
    {
        if(value>=enemyPool.boss.Count){
            value = enemyPool.boss.Count-1;
        }

        int spawnID = enemyPool.boss[value].ids[Random.Range(0, enemyPool.boss[value].ids.Count)];

        _enemySpawner.CreateEnemy(spawnID, (map.mapHeight+1)/2, (map.mapWidth+1)/2);

        BGMController.Instance.PlayByBGMType(BGMType.ChallengeBoss);

    }   
    //一般ウェーブの場合、ランダムパターンで生成
    private void GenerateEnemies(int value)
    {
       if(value>=enemyPool.enemies.Count){
            value = enemyPool.enemies.Count-1;
        }

        List<Vector2> cells = PickPattern(value).GetCells(grid);

        for(int i = 0; i < cells.Count; i++){
            int spawnID = enemyPool.enemies[value].ids[Random.Range(0, enemyPool.enemies[value].ids.Count)];
            _enemySpawner.CreateEnemy(spawnID, (int)cells[i].x, (int)cells[i].y);

        }
        BGMController.Instance.PlayByBGMType(BGMType.ChallengeNormal);

    }

    private ISpawnPattern PickPattern(int tier)
    {
        var pool = spawnPatterns.FindAll(pattern => pattern.tier <= tier);
        int pick = Random.Range(0, pool.Count);

        return pool[pick].pattern;
    }
}
