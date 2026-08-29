using UnityEngine;
using System.Collections.Generic;

//CollisionManagerに提供。リストを参照可能
public interface IEntityProvider
{
    IReadOnlyList<Enemy> ActiveEnemies { get;}
    IReadOnlyList<Shot> PlayerShots {get;}
    IReadOnlyList<Shot> EnemyShots {get;}
    Player ActivePlayer {get;}
    IReadOnlyList<DropItem> ActiveItems {get;}
    IReadOnlyList<ExpItem> ExpItems {get;}
}
//敵オブジェクトに提供。敵リストを更新可能
public interface IEnemyEntityUpdater
{
    void RemoveEnemy(Enemy _enemy);
    void CreateExpItem(Vector3 pos, float exp);
    void CreateEnemyShot(int id, Vector3 spawnPos, int enemyAttack, Vector3 shotDir);
    Shot CreateEnemyHitBox(int id, Vector3 spawnPos, int enemyAttack);
    void CreateEnemyAtPosition(int enemyID, Vector3 pos);
}
//プレイヤに提供する機能
//プレイヤ弾丸更新・近い敵返却
public interface IPlayerCombatHelper
{
    void CreatePlayerShot(int id, Vector3 spawnPos, int playerAttack, Vector3 shotDir);
    Enemy GetNearestTarget(Vector3 playerPos, float maxAttackRange);
}
//WaveManagerに提供
public interface IEnemySpawner
{
    void CreateEnemy(int enemyID, int x_, int y_);
    void CheckAllEnemiesDead();
    void CreateDropItem(int itemID, int x_, int y_);
}
//Shotに渡す
public interface IShotManager
{
    void CreatePlayerShot(int id, Vector3 spawnPos, int playerAttack, Vector3 shotDir);
    void CreateEnemyShot(int id, Vector3 spawnPos, int enemyAttack, Vector3 shotDir);
    void RemoveEnemyShot(Shot _shot);
    void RemovePlayerShot(Shot _shot);
}

public interface IExpOrbRemover
{
    void RemoveExpItem(ExpItem exp);
}

public interface IDropItemSystem
{
    DropItem CreateItemAtPosition(int itemID, Vector3 pos);
    void RemoveDropItem(DropItem item);
}