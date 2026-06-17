using UnityEngine;

public class LineShot : Shot
{
    [SerializeField] private int shotID;
    [SerializeField] private int shotCount = 2;
    [SerializeField] private float shotInterval = 0.3f;
    [SerializeField] private float attackDivide = 0.6f;

    public override void HandleShotAction(float deltaTime)
    {
        CreateLineShots();
        OnDie();
    }

    public override void OnAttack()
    {
        OnDie();
    }

    private void CreateLineShots()
    {

        float startOffset = -(shotCount - 1) * shotInterval/2.0f;

        for(int i = 0; i < shotCount; i++)
        {
            float offset = startOffset + (i * shotInterval);
            Vector3 spawnPos = transform.position + moveDirection * offset;

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
