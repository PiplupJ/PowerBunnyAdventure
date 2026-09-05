using UnityEngine;
//距離計算
public static class DistanceHelper
{
    public static bool IsInRange(Vector3 from, Vector3 to, float range)
    {
        return GetDistSqrXZ(from, to) <= range * range;
    }

    public static float GetDistSqrXZ(Vector3 from, Vector3 to)
    {
        float distX = from.x - to.x;
        float distZ = from.z - to.z;

        return distX * distX + distZ * distZ;
    }

}
