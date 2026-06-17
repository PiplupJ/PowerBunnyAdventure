using UnityEngine;

public class WhirlwindBehaviour : ChargeBehaviour
{
    [SerializeField] private int _shotID;
    protected Vector3 shotDir;

    [SerializeField] private float firePointOffset = 3.0f;

    protected override void OnChargeStart()
    {
        SpawnHitBox();
        SpawnShot();
    }

    public void SpawnShot()
    {
        _enemy._enemyUpdater.CreateEnemyShot(_shotID, GetFirePoint(), _enemy.stat.attack, shotDir);
    }

    public Vector3 GetFirePoint()
    {
        return _enemy.transform.position + 
               _enemy.transform.forward * firePointOffset;
    }
}
