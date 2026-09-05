using UnityEngine;
using System.Collections;
using System;
//ワールドマップ画面用プレイヤー
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
    //移動モーションは目標点に近づいたら徐々に遅くなる
    //走り
    private void MoveRun(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * runSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(1.0f);

        RotateTo(dir);
    }
    //歩き
    private void MoveWalk(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * walkSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(0.5f);

        RotateTo(dir);
    }
    //徐行
    private void MoveSlow(Vector2 dir)
    {
        Vector2 moveVec = dir * Time.deltaTime * stopSpeed;//移動量計算

        transform.position += new Vector3(moveVec.x, 0f, moveVec.y);
        
        animationController.UpdateBlendMotion(0.0f);

        RotateTo(dir);
    }
    //止まる
    private void MoveStop()
    {
        animationController.StopBlendMotion();
        RotateToDefault();
    }
    //進んでいる向きへ
    protected void RotateTo(Vector2 rotDir)
    {
        if(rotDir.x == 0 && rotDir.y == 0) { return; }

        transform.rotation = MovementHelper.GetRoation(rotDir);
    }
    //基本向きへ
    public void RotateToDefault()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public IEnumerator StartRoutine(Action onComplete)
    {
        string animationTag = "Ready";
        animationController.PlayAction(animationTag);

        float timeout = 3f;

        while(animationController.GetNormalizedTime(animationTag)<0.5f && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null; 
        }

        weapon.SetActive(true);

        timeout = 3f;
        while(animationController.GetNormalizedTime(animationTag)>= 1.0f && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null; 
        }

        onComplete?.Invoke();
    }
}
