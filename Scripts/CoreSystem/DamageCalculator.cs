using UnityEngine;

public static class DamageCalculator
{
    //ダメージを求める
    public static int GetFinalDamage(int attack, int defense)
    {
        float damage = attack * (100.0f / (100.0f + defense));

        return GetRollDamage(damage);
    }

    //ダメージに乱数適用
    private static int GetRollDamage(float baseDamage)
    {
        float minDamage = baseDamage * 0.95f;
        float maxDamage = baseDamage * 1.05f;

        float rollResult = Random.Range(minDamage, maxDamage);

        return (int)(rollResult + 0.5f);
    }

    //クリティカルヒットだったか
    public static bool IsCriticalHit(float criticalRate)
    {
        return Random.value <= criticalRate;
    }
}
