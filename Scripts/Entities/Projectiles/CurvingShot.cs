using UnityEngine;
//曲がる弾
public class CurvingShot : Shot
{

    [SerializeField] private float _curveAngle = 45.0f;
    [SerializeField] private float _curveFrequency = 0.5f;
    private float timer;
    private float _curveDirection;

    private void OnEnable()
    {
        timer = 0.0f;
    }

    public override void FireShot(int shooterAttack, Vector3 shotDir)
    {
        base.FireShot(shooterAttack, shotDir);

        float angle = Mathf.Atan2(shotDir.x, shotDir.z) * Mathf.Rad2Deg;

        _curveDirection = angle >= 0f ? 1f: -1f;
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(_mapCollision.MapWallHitCheck(this.transform.position, shotData.rad)){
            OnDie();
            return;
        }
        //曲がる時間になったら曲がる
        if(timer >= _curveFrequency)
        {
            float rotateAngle = _curveAngle * Mathf.Deg2Rad * _curveDirection;

            float cos = Mathf.Cos(-rotateAngle);
            float sin = Mathf.Sin(-rotateAngle);

            moveDirection = new Vector3(
                    moveDirection.x * cos - moveDirection.z*sin,
                    0f,
                    moveDirection.x * sin + moveDirection.z*cos
            ).normalized;

            timer = 0;
        }

        prevPos = transform.position;
        Vector3 moveVec = shotData.moveSpeed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;

        timer += deltaTime;

    }
    
    public override void OnAttack()
    {
        SpawnHitEffect(new Vector3(transform.position.x, 1, transform.position.z));
        OnDie();
    }
}
