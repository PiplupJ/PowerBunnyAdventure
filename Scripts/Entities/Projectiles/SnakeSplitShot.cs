using UnityEngine;

//ジグザグで動き、分裂する弾
public class SnakeSplitShot : Shot
{
    [SerializeField] private float _curveAngle = 45.0f;
    [SerializeField] private float _curveFrequency = 0.5f;

    private float curveTimer;
    private float _rotateAngle;
    private int     _curveStep;
    private Vector3 _originalDir;

    [SerializeField] private float splitTime = 1.5f;
    [SerializeField] private int splitCount;
    [SerializeField] private int shotID;
    private float splitTimer;
    
    private void OnEnable()
    {
        curveTimer = 0.0f;
        splitTimer = 0.0f;
        _curveStep = 0;
    }

    public override void FireShot(int shooterAttack, Vector3 shotDir)
    {
        base.FireShot(shooterAttack, shotDir);

        _originalDir = shotDir;
        _rotateAngle = _curveAngle * Mathf.Deg2Rad;
        _curveStep   = 0;
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            Split();
            return;
        }

        if(curveTimer >= _curveFrequency)
        {
            RenewMoveDirection();
            curveTimer = 0;
        }

        if(splitTimer >= splitTime)
        {
            Split();
        }

        prevPos = transform.position;
        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;

        curveTimer += deltaTime;
        splitTimer += deltaTime;

    }
    
    public override void OnAttack()
    {
        SpawnHitEffect(new Vector3(transform.position.x, 1, transform.position.z));
        OnDie();
    }

    private void RenewMoveDirection()
    {
        _curveStep++;
        float sign  = (_curveStep % 2 == 0) ? 1f : -1f;
        float angle = _rotateAngle * sign;

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        moveDirection = new Vector3(
            _originalDir.x * cos - _originalDir.z * sin,
            0f,
            _originalDir.x * sin + _originalDir.z * cos
        ).normalized;
    }

    private void Split()
    {
        float baseAngle = Mathf.Atan2(moveDirection.x, moveDirection.z);

        for(int i = 0; i < splitCount; i++)
        {
            float angle = i * (360f / splitCount) * Mathf.Deg2Rad;
            float finalAngle = baseAngle + angle;
            Vector3 dir = new Vector3(Mathf.Cos(finalAngle), 0f, Mathf.Sin(finalAngle));

            if(_isPlayerShot){
            _shotManager.CreatePlayerShot(shotID, transform.position, attack, dir);
            }
            else{
                _shotManager.CreateEnemyShot(shotID, transform.position, attack, dir);
            }
        }
        OnDie();
    }
}
