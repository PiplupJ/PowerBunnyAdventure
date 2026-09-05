using UnityEngine;

//壁にめり込まない処理などに使用
public class MoveResult
{
    public bool hitwallX;
    public bool hitwallZ;
    public Vector2 moveVec;
}

public static class MovementHelper
{
    //壁にめり込まないように移動量調整
    public static Vector2 CheckMove(Vector3 currentPos, Vector2 moveVec, float rad, IMapCollision mapCollision)
    {
        if(moveVec.x != 0)
        {
            Vector3 nextPosX = new Vector3(currentPos.x + moveVec.x, currentPos.y, currentPos.z);

            if(mapCollision.MapWallHitCheck(nextPosX, rad))
            {
                moveVec.x = BinarySearchToWall(currentPos, moveVec.x, true, rad, mapCollision);
            }
        }

        if(moveVec.y != 0)
        {
            Vector3 nextPosY = new Vector3(currentPos.x, currentPos.y, currentPos.z  + moveVec.y);

            if(mapCollision.MapWallHitCheck(nextPosY, rad))
            {
                moveVec.y = BinarySearchToWall(currentPos, moveVec.y, false, rad, mapCollision);
            }
        }

        return moveVec;
    }   
    //バイナリサーチで壁にめり込まないようにする
    private static float BinarySearchToWall(Vector3 currentPos, float moveValue, bool isX, float rad, IMapCollision mapCollision)
    {
        float sign = Mathf.Sign(moveValue);
        float low = 0.0f;
        float high = Mathf.Abs(moveValue);

        for(int i = 0; i < 8; i++)
        {
            float mid = (low + high) / 2.0f;

            Vector3 testPos = isX 
                ? new Vector3(currentPos.x + mid * sign, currentPos.y, currentPos.z)
                : new Vector3(currentPos.x, currentPos.y, currentPos.z + mid * sign);

                if (mapCollision.MapWallHitCheck(testPos, rad))
                {
                    high = mid;  // 
                }
                else
                {
                    low  = mid;  // 
                }
        }
        return low * sign;
    }
    //回転角度
    public static float GetRotationAngle(Vector2 direction)
    {
        if(direction.x == 0f && direction.y ==0f) { return 0f; }
        return Mathf.Atan2(direction.x, direction.y)* Mathf.Rad2Deg;
    }

    public static Quaternion GetRoation(Vector2 direction)
    {
        return Quaternion.Euler(0f, GetRotationAngle(direction), 0f);
    }
    //どの軸に壁があったかが必要な時使用
    public static MoveResult CheckMoveInDetail(Vector3 currentPos, Vector2 moveVec, float rad, IMapCollision mapCollision)
    {
        bool collisionX = false;
        bool collisionZ = false;

        if(moveVec.x != 0)
        {
            Vector3 nextPosX = new Vector3(currentPos.x + moveVec.x, currentPos.y, currentPos.z);

            if(mapCollision.MapWallHitCheck(nextPosX, rad))
            {
                moveVec.x = BinarySearchToWall(currentPos, moveVec.x, true, rad, mapCollision);
                collisionX = true;
            }
        }

        if(moveVec.y != 0)
        {
            Vector3 nextPosY = new Vector3(currentPos.x, currentPos.y, currentPos.z  + moveVec.y);

            if(mapCollision.MapWallHitCheck(nextPosY, rad))
            {
                moveVec.y = BinarySearchToWall(currentPos, moveVec.y, false, rad, mapCollision);
                collisionZ = true;
            }
        }

        MoveResult res = new MoveResult();
        res.hitwallX = collisionX;
        res.hitwallZ = collisionZ;
        res.moveVec = moveVec;

        return res;
    }

}
