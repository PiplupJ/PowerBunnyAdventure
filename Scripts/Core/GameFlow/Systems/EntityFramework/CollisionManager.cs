using UnityEngine;

public class CollisionManager
{
    private IEntityProvider entityProvider;

    public CollisionManager()
    {
    }
    //初期化
    public void Init(IEntityProvider entityProvider)
    {
        this.entityProvider = entityProvider;
    }
    //GameManagerで実行
    public void HandleCollisions()
    {
        PlayerShotCollision();
        EnemyShotCollision();
        ItemCollision();
    }
    //経験値アイテムは、ステージ終了後実行するために別
    public void HandleExpItemCollisions()
    {
        ExpItemCollision();
    }

    //プレイヤー弾と敵の衝突判定
    private void PlayerShotCollision()
    {   
        //弾丸がないなら実行しない
        if(entityProvider.PlayerShots.Count == 0) { return; }
        if(entityProvider.ActiveEnemies.Count == 0) { return;}
        //弾丸と敵の当たり判定
        for(int i = entityProvider.PlayerShots.Count - 1; i >=0; i--)
        {
            Shot pShot = entityProvider.PlayerShots[i];

            for(int j = entityProvider.ActiveEnemies.Count - 1; j>=0; j--)
            {
                Enemy enemy = entityProvider.ActiveEnemies[j];

                //敵が無くなった状態ならパス
                if(enemy.GetState()!=EnemyState.Active) { continue; }
                //敵が隠しているか
                if(enemy.IsStealthed) { continue; }

                float combinedRad = pShot.shotData.rad + enemy.stat.rad;
                //衝突したか
                if(HadCollision(pShot.transform.position, enemy.transform.position, combinedRad))
                {
                    //この弾にもう攻撃されたか
                    if (!pShot.TryHit(enemy.GetInstanceID())) continue;
                    //クリティカル攻撃だったか
                    bool wasCritical = DamageCalculator.IsCriticalHit(entityProvider.ActivePlayer.stat.criticalRate.value);
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
    //敵の弾とプレイヤーの衝突判定
    private void EnemyShotCollision()
    {
        //弾丸がないなら実行しない
        if(entityProvider.EnemyShots.Count == 0) { return; }

        Player player = entityProvider.ActivePlayer;

        for(int i = entityProvider.EnemyShots.Count - 1; i >=0; i--)
        {
            Shot eShot = entityProvider.EnemyShots[i];

            float combinedRad = eShot.shotData.rad + player.stat.rad;
            //衝突したか
            if(HadCollision(eShot.transform.position, player.transform.position, combinedRad))
            {
                //この弾にもう攻撃されたか
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
    //プレイヤーとアイテムの衝突判定
    private void ItemCollision()
    {
        if(entityProvider.ActiveItems.Count==0) { return; }

        Player player = entityProvider.ActivePlayer;

        for(int i = entityProvider.ActiveItems.Count - 1; i >=0; i--)
        {
            DropItem item = entityProvider.ActiveItems[i];

            float combinedRad = item.rad + player.stat.rad;

            if(HadCollision(item.transform.position, player.transform.position, combinedRad))
            {
                item.ApplyEffect();
                break;
            }
        }
    }
    //プレイヤーと経験値オブの衝突判定
    private void ExpItemCollision()
    {
        if(entityProvider.ExpItems.Count==0) { return; }

        Player player = entityProvider.ActivePlayer;

        for(int i = entityProvider.ExpItems.Count - 1; i >=0; i--)
        {
            ExpItem exp = entityProvider.ExpItems[i];

            float combinedRad = exp.rad + player.stat.rad;

            if(HadCollision(exp.transform.position, player.transform.position, combinedRad))
            {
                player.levelUpSystem.ExpCalculation(exp.rewardExp);
                exp.ApplyEffect();
            }
        }
    }
    //衝突したかを判定
    private bool HadCollision(Vector3 posA, Vector3 posB, float combinedRad)
    {
        Vector3 distVec = posA - posB;
        float distSqr = distVec.x * distVec.x + distVec.y * distVec.y + distVec.z * distVec.z;
        float limitDist = combinedRad * combinedRad;

        return(distSqr <= limitDist);
    }
}
