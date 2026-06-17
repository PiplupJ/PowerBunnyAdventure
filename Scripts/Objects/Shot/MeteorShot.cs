using UnityEngine;

public class MeteorShot : Shot
{
    [SerializeField] private float _fallHeight = 8f;  
    [SerializeField] private float _hitHeight  = 0.3f; 
    [SerializeField] private float _accSpeed  = 0.163f; 
    [SerializeField] private int   hazardID; 

    float speed;

    public override void FireShot(int shooterAttack, Vector3 shotDir)
    {
        this.attack = shooterAttack;
        this.transform.position = new Vector3(this.transform.position.x, _fallHeight, this.transform.position.z);
        this.moveDirection = new Vector3(0, -1, 0);
        speed = this.shotData.moveSpeed;
    } 

    public override bool TryHit(int instanceID)
    {
        return false;
    }

    public override void HandleShotAction(float deltaTime)
    {
        if(this.transform.position.y <= this._hitHeight){
            OnAttack();
            return;
        }
        
        prevPos = transform.position;
        Vector3 moveVec = this.speed * moveDirection * deltaTime;
        Vector3 nextPos = prevPos + moveVec;
        this.transform.position = nextPos;

        this.speed += this._accSpeed;
    }

    public override void OnAttack()
    {
        this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        CreateHazard();
    }

    private void CreateHazard()
    {
        if(_isPlayerShot){
            _shotManager.CreatePlayerShot(hazardID, transform.position, attack, Vector3.zero);
        }
        else{
            _shotManager.CreateEnemyShot(hazardID, transform.position, attack, Vector3.zero);
        }
        OnDie();
    }

}
