using UnityEngine;

public class StraightMultiShot : Shot
{
    [SerializeField] private int shotID = IDRegistry.BASE_SHOT;
    [SerializeField] private int shotCount = 2;
    [SerializeField] private float shotInterval = 0.3f;
    [SerializeField] private float attackDivide = 0.6f;

    public override void HandleShotAction(float deltaTime)
    {
        CreateMultiShots();
        OnDie();
    }

    public override void OnAttack()
    {
        OnDie();
    }

    private void CreateMultiShots()
    {
        Vector3 rightDir = new Vector3(-moveDirection.z, 0f, moveDirection.x);

        float startOffset = (-shotCount - 1) * shotInterval/2.0f;

        for(int i = 0; i < shotCount; i++)
        {
            float offset = startOffset + (i * shotInterval);
            Vector3 spawnPos = transform.position + rightDir * offset;

            int multiShotAttack = (int)(attack * attackDivide);

            if(_isPlayerShot){
            _shotManager.CreatePlayerShot(shotID, spawnPos, multiShotAttack, moveDirection);
            }
            else{
                _shotManager.CreateEnemyShot(shotID, spawnPos, multiShotAttack, moveDirection);
            }
        }
    }

}
