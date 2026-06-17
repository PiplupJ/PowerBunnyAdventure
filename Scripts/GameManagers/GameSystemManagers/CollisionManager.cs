using UnityEngine;

public class CollisionManager
{
    private IEntityProvider _entityProvider;

    public CollisionManager()
    {
    }
    //初期化
    public void Init(IEntityProvider newProvider)
    {
        _entityProvider = newProvider;
    }

    public void HandleCollisions()
    {
        PlayerShotCollision();
        EnemyShotCollision();
        ItemCollision();
    }

    public void HandleExpItemCollisions()
    {
        ExpItemCollision();
    }

    private void PlayerShotCollision()
    {   
        //弾丸がないなら実行しない
        if(_entityProvider.PlayerShots.Count == 0) { return; }
        //弾丸と敵の当たり判定
        for(int i = _entityProvider.PlayerShots.Count - 1; i >=0; i--)
        {
            Shot pShot = _entityProvider.PlayerShots[i];

            for(int j = _entityProvider.ActiveEnemies.Count - 1; j>=0; j--)
            {
                Enemy enemy = _entityProvider.ActiveEnemies[j];

                float combinedRad = pShot.shotData.rad + enemy.stat.rad;

                if(hadCollision(pShot.transform.position, enemy.transform.position, combinedRad))
                {
                    if(enemy.IsStealthed) { continue; }

                    if (!pShot.TryHit(enemy.GetInstanceID())) continue;
                    //クリティカル攻撃だったか
                    bool wasCritical = DamageCalculator.IsCriticalHit(_entityProvider.ActivePlayer.stat.criticalRate.value);
                    //弾丸を最終ダメージを求めて、敵のダメージ判定を呼び出す。
                    int finalDamage;
                    if(wasCritical)
                    {
                        int criticalDamage = (int)(pShot.attack * 1.5f);
                        finalDamage = DamageCalculator.GetFinalDamage(criticalDamage, 0);
                        //クリティカル攻撃は防御力を無視
                        
                    }
                    else {
                        finalDamage = DamageCalculator.GetFinalDamage(pShot.attack, enemy.stat.defense);
                    }
                    enemy.OnHit(finalDamage, wasCritical);
                    //弾丸の攻撃処理。エフェクトなど発生
                    pShot.OnAttack();
                    break;
                }
            }
        }
    }

    private void EnemyShotCollision()
    {
        //弾丸がないなら実行しない
        if(_entityProvider.EnemyShots.Count == 0) { return; }

        Player player = _entityProvider.ActivePlayer;

        for(int i = _entityProvider.EnemyShots.Count - 1; i >=0; i--)
        {
            Shot eShot = _entityProvider.EnemyShots[i];

            float combinedRad = eShot.shotData.rad + player.stat.rad;

            if(hadCollision(eShot.transform.position, player.transform.position, combinedRad))
            {
                if (!eShot.TryHit(player.GetInstanceID())) continue;
                //弾丸を最終ダメージを求めて、敵のダメージ判定を呼び出す。
                int finalDamage = DamageCalculator.GetFinalDamage(eShot.attack, player.stat.defense.value);
                
                player.OnHit(finalDamage, false);
                //弾丸の攻撃処理。エフェクトなど発生
                eShot.OnAttack();
                break;
            }        
        }
    }

    private void ItemCollision()
    {
        if(_entityProvider.ActiveItems.Count==0) { return; }

        Player player = _entityProvider.ActivePlayer;

        for(int i = _entityProvider.ActiveItems.Count - 1; i >=0; i--)
        {
            DropItem item = _entityProvider.ActiveItems[i];

            float combinedRad = item.rad + player.stat.rad;

            if(hadCollision(item.transform.position, player.transform.position, combinedRad))
            {
                item.ApplyEffect();
                break;
            }
        }
    }

    private void ExpItemCollision()
    {
        if(_entityProvider.ExpItems.Count==0) { return; }

        Player player = _entityProvider.ActivePlayer;

        for(int i = _entityProvider.ExpItems.Count - 1; i >=0; i--)
        {
            ExpItem exp = _entityProvider.ExpItems[i];

            float combinedRad = exp.rad + player.stat.rad;

            if(hadCollision(exp.transform.position, player.transform.position, combinedRad))
            {
                player._levelUpSystem.ExpCalculation(exp.rewardExp);
                exp.ApplyEffect();
            }
        }
    }
    
    private bool hadCollision(Vector3 posA, Vector3 posB, float combinedRad)
    {
        Vector3 distVec = posA - posB;
        float distSqr = distVec.x * distVec.x + distVec.y * distVec.y + distVec.z * distVec.z;
        float limitDist = combinedRad * combinedRad;

        return(distSqr <= limitDist);
    }
}
