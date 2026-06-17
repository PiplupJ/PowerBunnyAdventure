using UnityEngine;

public class WallAwareChaseBehaviour : ChaseBehaviour
{
    [SerializeField] private float _probeAngle   = 45f; 
    [SerializeField] private int   _probeCount   = 8;  

    public override void OnChase(float fixedDeltaTime)
    {
        _enemy.animationController.UpdateBlendMotion(speed);
        Vector3 dir = (_enemy.target.position - _enemy.transform.position).normalized;

        Vector2 moveVec = FindBestRoute(dir, fixedDeltaTime);

        _enemy.RotateToTarget(moveVec.normalized);
        _enemy.transform.position += new Vector3(moveVec.x, 0.0f, moveVec.y);
    }

    private Vector2 FindBestRoute(Vector3 toTarget, float fixedDeltaTime)
    {
        
        Vector2 originalDir = new Vector2(toTarget.x, toTarget.z);
        Vector2 moveVec = originalDir * _enemy.stat.moveSpeed * fixedDeltaTime;

        moveVec = MovementHelper.CheckMove(transform.position, moveVec, moveRad, _enemy._mapCollision);

        if(moveVec.x * moveVec.x + moveVec.y * moveVec.y> 0.0001)
        {
            return moveVec;
        }

        Vector2 bestMoveVec  = Vector2.zero;
        float   minDistSqr  = float.MaxValue;
        Vector3 targetPos    = _enemy.target.position;

        for(int i = 1; i <= _probeCount; i++)
        {
            float angle = _probeAngle * i * Mathf.Deg2Rad;

            Vector2 right = GetRotatedVector(originalDir, angle) * _enemy.stat.moveSpeed * fixedDeltaTime;
            
            TryDirection(right, targetPos, ref bestMoveVec, ref minDistSqr);

            Vector2 left = GetRotatedVector(originalDir, -angle) * _enemy.stat.moveSpeed * fixedDeltaTime;
            
            TryDirection(left, targetPos, ref bestMoveVec, ref minDistSqr);

        }
        return bestMoveVec;
    }

    private void TryDirection(Vector2 moveVec, Vector3 targetPos, ref Vector2 bestMoveVec, ref float minDistSqr)
    {
        Vector2 res = MovementHelper.CheckMove(
            _enemy.transform.position, moveVec,
            moveRad, _enemy._mapCollision);

        if(res.x * res.x + res.y * res.y <= 0.0001)
        {
            return;
        }

        Vector3 nextPos =  _enemy.transform.position + new Vector3(res.x, 0.0f, res.y);

        float currentDistSqr = DistanceHelper.GetDistSqrXZ(nextPos, targetPos);

            if(currentDistSqr < minDistSqr)
            {
                minDistSqr  = currentDistSqr;
                bestMoveVec  = res;
            }
    }

    private Vector2 GetRotatedVector(Vector2 vec, float angle)
    {
        return new Vector2(
            vec.x * Mathf.Cos(angle) - vec.y * Mathf.Sin(angle),
            vec.x * Mathf.Sin(angle) + vec.y * Mathf.Cos(angle));
    }
}
