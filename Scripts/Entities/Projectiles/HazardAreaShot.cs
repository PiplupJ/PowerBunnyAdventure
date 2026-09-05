using UnityEngine;

public class HazardAreaShot : Shot
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float dotInterval = 1.0f;
    private float timer = 0.0f;
    private float dotTimer = 0.0f;

    public override bool TryHit(int instanceID)
    {
        if(alreadyHit.Contains(instanceID)) { return false; }
        alreadyHit.Add(instanceID);
        dotTimer = dotInterval;
        return true;
    }

    private void OnEnable()
    {
        timer = lifeTime;
        
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(timer<=0)
        {
            OnDie();
        }
        if(dotTimer>0){
            dotTimer -= deltaTime;
            if(dotTimer<=0){
                alreadyHit.Clear();
            }
        }
        timer -= deltaTime;
    }

    public override void OnAttack()
    {
    }

}
