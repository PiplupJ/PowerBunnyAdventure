using UnityEngine;
using System.Collections;
using System;

public class WorldMapPlayer : MonoBehaviour
{
    public PlayerAnimationController animationController;

    public GameObject weapon;

    public float runSpeed = 3.0f;
    public float walkSpeed = 1.0f;
    public float stopSpeed = 0.5f;

    public IEnumerator MoveToNextWorld(Vector3 target, Action onComplete)
    {
        yield return StartCoroutine(MoveRoutine(target));
        onComplete?.Invoke();
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        while(true)
        {
            Vector3 distVec = target - transform.position;
            float distSqr = distVec.x * distVec.x + distVec.y * distVec.y + distVec.z * distVec.z;

            Vector3 dir = distVec.normalized;

            if(distSqr <= 0.1f)
            {
                transform.position = target;
                MoveStop();
                break;
            }
            else if (distSqr <= 0.3f)
            {
                MoveSlow(new Vector2(dir.x, dir.z));
            }
            else if(distSqr <= 2.0f)
            {
                MoveWalk(new Vector2(dir.x, dir.z));
            }
            else
            {
                MoveRun(new Vector2(dir.x, dir.z));
            }
         
            yield return null;
        }
    }

    private void MoveRun(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * runSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(1.0f);

        RotateTo(dir);
    }

    private void MoveWalk(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * walkSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(0.5f);

        RotateTo(dir);
    }

    private void MoveSlow(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * stopSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(0.0f);

        RotateTo(dir);
    }

    private void MoveStop()
    {
        animationController.StopBlendMotion();
        RotateToDefault();
    }

    protected void RotateTo(Vector2 rotDir)
    {
        if(rotDir.x == 0 && rotDir.y == 0) { return; }

        transform.rotation = MovementHelper.GetRoation(rotDir);
    }

    public void RotateToDefault()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public IEnumerator StartRoutine(Action onComplete)
    {
        string animationTag = "Ready";
        animationController.PlayAction(animationTag);

        while(animationController.GetNormalizedTime(animationTag)<0.5f)
        {
            yield return null; 
        }

        weapon.SetActive(true);

        while(animationController.GetNormalizedTime(animationTag)>= 1.0f)
        {
            yield return null; 
        }

        onComplete?.Invoke();
    }
}
