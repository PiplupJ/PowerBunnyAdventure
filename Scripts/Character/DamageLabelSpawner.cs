using UnityEngine;

public static class DamageLabelSpawner
{
    //ダメージラベルID
    const int damageLabelID = 6000000;
    
    public static void ShowDamage(int damage, bool wasCrit, Vector3 pos)
    {
        DamageLabel damageLabel = ObjectPool.Instance.GetObject<DamageLabel>(damageLabelID);
        damageLabel.Init(damage, wasCrit, pos);
    }

    public static void ShowStatusDamage(int damage, StatusType type, Vector3 pos)
    {

    }
}
