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
    private List<Enemy> _activeEnemies;
    private List<Shot> _playerShots;
    private List<Shot> _enemyShots;
    private Player _player;
    private List<DropItem> _activeItems;
    private List<ExpItem> _expItems;

    public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
    public IReadOnlyList<Shot> PlayerShots => _playerShots;
    public IReadOnlyList<Shot> EnemyShots => _enemyShots;
    public Player ActivePlayer => _player;
    public IReadOnlyList<DropItem> ActiveItems => _activeItems;
    public IReadOnlyList<ExpItem> ExpItems => _expItems;

    public event Action AllEnemiesDead;

    private IMapSystem _mapSystem;

    private ShotDataManager shotDataManager;
    private EnemyDataManager enemyDataManager;
    
    private ExpDropSystem _expDropSystem; 

    //初期化
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

    public void Init(Player currentPlayer, IMapSystem newMapSystem)
    {
        _player = currentPlayer;
        _mapSystem = newMapSystem;
    }

    
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
    //敵を静止して配置。WaveManagerが呼び出す
    public void CreateEnemy(int enemyID, int x_, int y_)
    {
        Vector3 worldPos = _mapSystem.GridToWorldSpace(x_, y_);

        CreateEnemyAtPosition(enemyID, worldPos);
    }

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

    public void CheckAllEnemiesDead()
    {
        if(_activeEnemies.Count == 0)
        {
            Debug.Log("Enemies All Dead");
            AllEnemiesDead?.Invoke();
        }
    }

    public void CreatePlayerShot(int id, Vector3 spawnPos, int playerAttack, Vector3 shotDir)
    {
        Shot pShot = ObjectPool.Instance.GetObject<Shot>(id);
        pShot.transform.position = spawnPos;
        bool isPlayerShot = true;
        pShot.SetShotData(shotDataManager.GetShotDataByID(id), _mapSystem, this, isPlayerShot);
        pShot.FireShot(playerAttack, shotDir);
        AddPlayerShot(pShot);
    }

    public void AddPlayerShot(Shot _shot)
    {
        _playerShots.Add(_shot);
    }

    //プレイヤの弾丸をリストから除外
    public void RemovePlayerShot(Shot _shot)
    {
        _playerShots.Remove(_shot);
    }

    public void CreateEnemyShot(int id, Vector3 spawnPos, int enemyAttack, Vector3 shotDir)
    {
        Shot eShot = ObjectPool.Instance.GetObject<Shot>(id);
        eShot.transform.position = spawnPos;
        bool isPlayerShot = false;
        eShot.SetShotData(shotDataManager.GetShotDataByID(id), _mapSystem, this, isPlayerShot);
        eShot.FireShot(enemyAttack, shotDir);
        AddEnemyShot(eShot);
    }

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
    
    public void AddExpItem(ExpItem exp)
    {
        _expItems.Add(exp);
    }

    public void RemoveExpItem(ExpItem exp)
    {
        _expItems.Remove(exp);
    }

    public void CreateDropItem(int itemID, int x_, int y_)
    {
        Vector3 worldPos = _mapSystem.GridToWorldSpace(x_, y_);

        CreateItemAtPosition(itemID, worldPos);
    }

    public DropItem CreateItemAtPosition(int itemID, Vector3 pos)
    {
        DropItem item = ObjectPool.Instance.GetObject<DropItem>(itemID);
        item.Init(this, _player);
        item.transform.position = pos;
        
        _activeItems.Add(item);

        return item;
    }

    public void RemoveDropItem(DropItem item)
    {
        _activeItems.Remove(item);
    }

    public void ClearAllEntities()
    {
        ClearList(_activeEnemies);
        ClearList(_playerShots);
        ClearList(_enemyShots);
        ClearList(_activeItems);
        ClearList(_expItems);
        
        Debug.Log("すべてのEntityリストを整理しました。");
    }

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
