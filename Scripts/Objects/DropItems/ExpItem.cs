using UnityEngine;

public class ExpItem : PoolableObject
{
    public float rewardExp;
    public float item;
    public float moveSpeed;
    public Transform _target;
    public float accel;

    public float rad = 1.0f; 

    public IExpOrbRemover expRemover;

    public void Init(float newExp, Transform newTarget, IExpOrbRemover newRemover)
    {
        rewardExp = newExp;
        _target = newTarget;
        expRemover = newRemover;
    }

    public void HandleItemAction(float deltaTime)
    {

        Vector3 currentPos = this.transform.position;
	    Vector3 moveDirection = (_target.position - currentPos).normalized;
	
	    Vector3 moveVec = moveDirection * moveSpeed * deltaTime;
	    
        this.transform.position = 
		    new Vector3(currentPos.x + moveVec.x, 
						currentPos.y + moveVec.y, 
						currentPos.z + moveVec.z);
	
	    moveSpeed += accel;
    }

    public void ApplyEffect()
    {
        expRemover.RemoveExpItem(this);
        ReturnToPool();
    }
}
