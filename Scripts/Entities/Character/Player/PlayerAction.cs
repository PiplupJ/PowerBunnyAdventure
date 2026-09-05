using UnityEngine;
using System.Collections;
using System;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    //ポータルに入る
    public IEnumerator EnterToPortal(Vector3 portalCenter, Action onComplete)
    {
        yield return StartCoroutine(MoveToPosition(portalCenter));
        onComplete?.Invoke();
        //入ったら予約された処理を行う
    }
    //ポータルから出る
    public IEnumerator ExitFromPortal(float goalDist, Action onComplete)
    {
        IPlayerControl player = playerController._currentPlayer;
        Vector3 target = new Vector3(player.playerPos.x, 0, player.playerPos.z + goalDist);

        yield return StartCoroutine(MoveToPosition(target));
        onComplete?.Invoke();
    }
    
    //指定された座標まで自動移動
    private IEnumerator MoveToPosition(Vector3 target)
    {
        IPlayerControl player = playerController._currentPlayer;
        float timeout = 3.0f;
        float elapsedTime = 0f;
        while(true)
        {
            Vector3 distVec = target - player.playerPos;
            float distSqr = distVec.x * distVec.x + distVec.y * distVec.y + distVec.z * distVec.z;

            if(distSqr <= 0.05f)
            {
                player.playerRoot.position = target;
                break;
            }

            elapsedTime += Time.deltaTime;
            //時間が過ぎたら自動的に目標座標へ
            if(elapsedTime > timeout)
            {
                player.playerRoot.position = target;
                break;
            }
            Vector3 dir = distVec.normalized;
            player.HandlePlayerInput(new Vector2(dir.x, dir.z));
            yield return null;
        }
    }    
}
