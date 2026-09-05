using UnityEngine;

public static class DamageCalculator
{
    private const float DEFENSE_CONSTANT = 100.0f;

    private const float ROLL_MIN = 0.95f;
    private const float ROLL_MAX = 1.05f;
    //ダメージを求める
    public static int GetFinalDamage(int attack, int defense)
    {
        float damage = attack * (DEFENSE_CONSTANT / (DEFENSE_CONSTANT + defense));

        return GetRollDamage(damage);
    }

    //ダメージに乱数適用
    private static int GetRollDamage(float baseDamage)
    {
        float minDamage = baseDamage * ROLL_MIN;
        float maxDamage = baseDamage * ROLL_MAX;

        float rollResult = Random.Range(minDamage, maxDamage);

        return (int)(rollResult + 0.5f);
    }

    //クリティカルヒットだったか
    public static bool IsCriticalHit(float criticalRate)
    {
        return Random.value <= criticalRate;
    }
}
