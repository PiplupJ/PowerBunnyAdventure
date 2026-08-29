using UnityEngine;
using System;
using Random = UnityEngine.Random;

public struct ExpDropInfo
{
    public Vector3 spawnPos;
    public float totalExp;
    public Transform target;
    public IExpOrbRemover remover;
}

public class ExpDropSystem
{
    private const float EXP_PER_ORB = 10;
    private const int MAX_ORBS = 10;
    private const int MIN_ORBS = 1;

    public void DropExp(ExpDropInfo dropInfo, Action<ExpItem> onOrbSpawned)
    {
        if (dropInfo.totalExp <= 0) return;
        
        float orbCount = Mathf.Clamp(dropInfo.totalExp / EXP_PER_ORB, MIN_ORBS, MAX_ORBS);
        float expPerOrb = dropInfo.totalExp / orbCount;
        float remainder = dropInfo.totalExp % orbCount; 
        
        for(int i = 0; i < orbCount; i++)
        {
            float orbExp = expPerOrb;
            if(i == 0) orbExp += remainder;

            ExpItem orb = ObjectPool.Instance.GetObject<ExpItem>(IDRegistry.EXP_ID);

            float xOffset = i == 0 ? 0f : Random.Range(-0.5f, 0.5f);
            float zOffset = i == 0 ? 0f : Random.Range(-0.5f, 0.5f);

            Vector3 orbSpawnPos = new Vector3(
                dropInfo.spawnPos.x + xOffset,
                dropInfo.spawnPos.y,
                dropInfo.spawnPos.z + zOffset);
            
            orb.transform.position = orbSpawnPos;
            orb.Init(orbExp, dropInfo.target, dropInfo.remover);

            onOrbSpawned?.Invoke(orb);
        }
    }
}
