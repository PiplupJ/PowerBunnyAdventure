using UnityEngine;
using System.Collections.Generic;
using System;

public class EntityManager : 
    IEntityProvider, 
    IEnemyEntityUpdater, 
    IPlayerCombatHelper, 
    IEnemySpawner, 
    IShotManager, 
    IExpOrbRemover, 
    IDropItemSystem
{
    //管理するオブジェクト種類別entityリスト
    private List<Enemy> _activeEnemies;
    private List<Shot> _playerShots;
    private List<Shot> _enemyShots;
    private Player _player;
    private List<DropItem> _activeItems;
    private List<ExpItem> _expItems;

    //参照用リスト。内容を変更できない
    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
    public IReadOnlyList<Shot> PlayerShots => _playerShots;
    public IReadOnlyList<Shot> EnemyShots => _enemyShots;
    public Player ActivePlayer => _player;
    public IReadOnlyList<DropItem> ActiveItems => _activeItems;
    public IReadOnlyList<ExpItem> ExpItems => _expItems;

    public event Action AllEnemiesDead; //敵殲滅

    private IMapSystem _mapSystem;  //マップ情報

    private ShotDataManager shotDataManager; //弾ステータス
    private EnemyDataManager enemyDataManager; //敵ステータス
    
    private ExpDropSystem _expDropSystem; 

    //生成
    public EntityManager()
    {
        _activeEnemies = new List<Enemy>();
        _playerShots = new List<Shot>();
        _enemyShots = new List<Shot>();
        _activeItems = new List<DropItem>();
        _expItems = new List<ExpItem>();

        shotDataManager = new ShotDataManager();
        shotDataManager.LoadShotDB();
        enemyDataManager = new EnemyDataManager();
        enemyDataManager.LoadEnemyDB();

        _expDropSystem = new ExpDropSystem();
        
        Debug.Log("EntityManager生成完了");
    }
    //初期化
    public void Init(Player currentPlayer, IMapSystem newMapSystem)
    {
        _player = currentPlayer;
        _mapSystem = newMapSystem;
    }

    //entityの状態更新
    public void HandleEntityState(float deltaTime)
    {
        for(int i = _activeEnemies.Count - 1; i >=0; i--)
        {
            _activeEnemies[i].ProcessAI(deltaTime);
        }
    }
    //物理的な更新。GameManagerのFixedUpdateで更新
    public void HandleEntityMovement(float fixedDeltaTime)
    {
        for(int i = _activeEnemies.Count - 1; i >=0; i--)
        {
            _activeEnemies[i].HandleEnemyAction(fixedDeltaTime);
        }
        ShotUpdate(fixedDeltaTime);
        for(int i = _activeItems.Count - 1; i >= 0; i--)
        {
            _activeItems[i].HandleItemAction(fixedDeltaTime);
        }
    }
    //GameClearFlagがOnになっている場合、GameManagerが実行
    public void HandleExpItemMovement(float fixedDeltaTime)
    {
        for(int i = _expItems.Count - 1; i >= 0; i--)
        {
            _expItems[i].HandleItemAction(fixedDeltaTime);
        }
    }
    //全ての弾丸の行動ロジックを更新
    private void ShotUpdate(float deltaTime)
    {
        for(int i = _playerShots.Count - 1; i >=0; i--)
        {
            _playerShots[i].HandleShotAction(deltaTime);
        }

        for(int i = _enemyShots.Count - 1; i >=0; i--)
        {
            _enemyShots[i].HandleShotAction(deltaTime);
        }
    }
    //敵をマップグリッドをもとに配置。WaveManagerが呼び出す
    public void CreateEnemy(int enemyID, int x_, int y_)
    {
        Vector3 worldPos = _mapSystem.GridToWorldSpace(x_, y_);

        CreateEnemyAtPosition(enemyID, worldPos);
    }
    //指定した座標に敵生成
    public void CreateEnemyAtPosition(int enemyID, Vector3 pos)
    {
        //ステータス、マップの当たり判定等の必要なインタフェース注入用変数
        EnemyInitContext eic = new EnemyInitContext
        {
            enemyData = enemyDataManager.GetEnemyDataByID(enemyID),
            mapCollision = _mapSystem,
            updater = this,
            target = _player.transform
        };
        //ObjectPoolから敵生成
        Enemy enemy = ObjectPool.Instance.GetObject<Enemy>(enemyID);
        //初期座標設定
        enemy.transform.position = pos;
        //敵を初期化
        enemy.InitStat(eic);
        //敵をEntityListに追加
        AddEnemy(enemy);
    }
    //entityリストに敵追加
    public void AddEnemy(Enemy _enemy)
    {
        _activeEnemies.Add(_enemy);
    }

    //死んだ敵をリストから除外
    public void RemoveEnemy(Enemy _enemy)
    {
        _activeEnemies.Remove(_enemy);
        
        CheckAllEnemiesDead();
    }
    //全ての敵が無くなったかを確認
    public void CheckAllEnemiesDead()
    {
        if(_activeEnemies.Count == 0)
        {
            Debug.Log("Enemies All Dead");
            AllEnemiesDead?.Invoke();
        }
    }

    //指定した座標にプレイヤーの弾生成
    public void CreatePlayerShot(int id, Vector3 spawnPos, int playerAttack, Vector3 shotDir)
    {
        Shot pShot = ObjectPool.Instance.GetObject<Shot>(id);
        pShot.transform.position = spawnPos;
        bool isPlayerShot = true;
        pShot.SetShotData(shotDataManager.GetShotDataByID(id), _mapSystem, this, isPlayerShot);
        pShot.FireShot(playerAttack, shotDir);
        AddPlayerShot(pShot);
    }
    //entityリストにプレイヤー弾追加
    public void AddPlayerShot(Shot _shot)
    {
        _playerShots.Add(_shot);
    }

    //プレイヤの弾丸をリストから除外
    public void RemovePlayerShot(Shot _shot)
    {
        _playerShots.Remove(_shot);
    }
    //敵の弾生成
    public void CreateEnemyShot(int id, Vector3 spawnPos, int enemyAttack, Vector3 shotDir)
    {
        Shot eShot = ObjectPool.Instance.GetObject<Shot>(id);
        eShot.transform.position = spawnPos;
        bool isPlayerShot = false;
        eShot.SetShotData(shotDataManager.GetShotDataByID(id), _mapSystem, this, isPlayerShot);
        eShot.FireShot(enemyAttack, shotDir);
        AddEnemyShot(eShot);
    }
    //敵の攻撃用ヒットボックス生成
    public Shot CreateEnemyHitBox(int id, Vector3 spawnPos, int enemyAttack)
    {
        Shot eHitBox = ObjectPool.Instance.GetObject<Shot>(id);
        eHitBox.transform.position = spawnPos;
        bool isPlayerShot = false;
        eHitBox.SetShotData(shotDataManager.GetShotDataByID(id), _mapSystem, this, isPlayerShot);
        eHitBox.InitAsHitBox(enemyAttack);
        AddEnemyShot(eHitBox);

        return eHitBox;
    }
    //entityリストに敵の弾を追加
    public void AddEnemyShot(Shot _shot)
    {
        _enemyShots.Add(_shot);
    }

    //敵の弾丸をリストから除外
    public void RemoveEnemyShot(Shot _shot)
    {
        _enemyShots.Remove(_shot);
    }

    //プレイヤが呼び出す。弾丸を発射する前、目標を決定
    //引数はプレイヤの座標、攻撃範囲
    public Enemy GetNearestTarget(Vector3 playerPos, float maxAttackRange)
    {
        //Debug.Log("現在の敵数:"+_activeEnemies.Count);
        if(_activeEnemies.Count == 0) { return null; }

        Enemy neareastEnemy = null;
        float minDistSqr = maxAttackRange * maxAttackRange;

        for(int i = _activeEnemies.Count - 1; i >=0; i--)
        {
            Enemy enemy = _activeEnemies[i];

            if(enemy.IsStealthed) { continue; }
            if(enemy.GetState()!=EnemyState.Active) { continue; }

            Vector3 enemyPos = enemy.transform.position;

            float currentDistSqr = DistanceHelper.GetDistSqrXZ(playerPos, enemy.transform.position);

            if(currentDistSqr < minDistSqr)
            {
                neareastEnemy = enemy;
                minDistSqr = currentDistSqr;
            }
        }
        //一番近い位置の敵を返却
        return neareastEnemy;
    }
    //経験値アイテム追加
    public void CreateExpItem(Vector3 pos, float exp)
    {
        ExpDropInfo edi = new ExpDropInfo
        {
            spawnPos = pos,
            totalExp = exp,
            target = _player.transform,
            remover = this
        };
        _expDropSystem.DropExp(edi, AddExpItem);
    }
    //経験値アイテムをentityリストに追加
    public void AddExpItem(ExpItem exp)
    {
        _expItems.Add(exp);
    }
    //経験値アイテムをリストから除外
    public void RemoveExpItem(ExpItem exp)
    {
        _expItems.Remove(exp);
    }
    //マップグリッドをもとにアイテム配置
    public void CreateDropItem(int itemID, int x_, int y_)
    {
        Vector3 worldPos = _mapSystem.GridToWorldSpace(x_, y_);

        CreateItemAtPosition(itemID, worldPos);
    }
    //指定された座標にアイテム配置
    public DropItem CreateItemAtPosition(int itemID, Vector3 pos)
    {
        DropItem item = ObjectPool.Instance.GetObject<DropItem>(itemID);
        item.Init(this, _player);
        item.transform.position = pos;
        
        _activeItems.Add(item);

        return item;
    }
    //アイテムをリストから除外
    public void RemoveDropItem(DropItem item)
    {
        _activeItems.Remove(item);
    }
    //Entityリストクリア
    public void ClearAllEntities()
    {
        ClearList(_activeEnemies);
        ClearList(_playerShots);
        ClearList(_enemyShots);
        ClearList(_activeItems);
        ClearList(_expItems);
        
        Debug.Log("すべてのEntityリストを整理しました。");
    }
    //全てのEntityをプールへ
    private void ClearList<T>(List<T> list ) where T : PoolableObject
    {
        foreach(var obj in list)
        {
            if(obj != null)
            {
                obj.ReturnToPool();
            }
        }
        list.Clear();
    }
}
