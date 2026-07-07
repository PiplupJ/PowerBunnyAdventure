using UnityEngine;
//ヘビのようにジグザグに動く弾
public class SnakeShot : Shot
{
    [SerializeField] private float _curveAngle = 45.0f;
    [SerializeField] private float _curveFrequency = 0.5f;

    private float timer;
    private float _rotateAngle;
    private int     _curveStep;
    private Vector3 _originalDir;
    
    private void OnEnable()
    {
        timer = 0.0f;
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
            OnDie();
            return;
        }

        if(timer >= _curveFrequency)
        {
            RenewMoveDirection();
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
        HitEffect effect = ObjectPool.Instance.GetObject<HitEffect>(shotData.hitEffectID);
        effect.transform.position = transform.position;
        OnDie();
    }
    //前とは反対向きに曲がる
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
}
