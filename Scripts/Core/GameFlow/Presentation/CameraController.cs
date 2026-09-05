using UnityEngine;

//カメラが映える範囲設定用
public struct CameraBounds
{
    public float left, right, top, bottom;

    public CameraBounds(float left, float right, float top, float bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}
public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] Transform target;
    [SerializeField] float cameraSpeed = 5.0f;
    [SerializeField] Vector3 cameraOffset; 

    private float _camHeight;
    private float _camWidth;

    private float _zLookAhead;  
    private float _zExtent; 

    private CameraBounds _bounds;
    private bool _isBoundSet = false;

    private void Awake()
    {
        if(mainCam == null){
            mainCam = Camera.main;
        }
        if(cameraOffset == Vector3.zero){
            cameraOffset = mainCam.transform.position;
            Debug.LogWarning("[CameraController]オフセットが未設定です");
        }
        _camHeight = mainCam.orthographicSize;
        _camWidth = _camHeight * mainCam.aspect;
        //angleRad : カメラのピッチ角
        //zLookAhead : カメラをoffset.y分上げた場合、地面の注視点が奥にずれる量
        //zExtent : 画面の縦半分を平面に撮影した長さ
        float angleRad = mainCam.transform.eulerAngles.x * Mathf.Deg2Rad;
        _zLookAhead = cameraOffset.y / Mathf.Tan(angleRad);  
        _zExtent    = _camHeight / Mathf.Sin(angleRad);
    }

    private void LateUpdate() 
    {
        if(target == null) { return; }

        Vector3 desiredPos = target.position + cameraOffset;
        Vector3 clampedPos;
        
        if(_isBoundSet)
        {
            clampedPos = GetClampedPos(desiredPos);
        }
        else{
            clampedPos = desiredPos;
        }
         

        transform.position = Vector3.Lerp(transform.position, clampedPos, cameraSpeed * Time.deltaTime);
    }

    public Vector3 GetClampedPos(Vector3 targetPos)
    {
        float clampedX = Mathf.Clamp(targetPos.x, _bounds.left + _camWidth, _bounds.right - _camWidth);
        //float clampedZ = Mathf.Clamp(targetPos.z, _bounds.bottom + _camHeight, _bounds.top - _camHeight);

        float minZ = _bounds.bottom - _zLookAhead + _zExtent;
        float maxZ = _bounds.top    - _zLookAhead - _zExtent;
        float clampedZ = Mathf.Clamp(targetPos.z, minZ, maxZ);
        
        return new Vector3(clampedX, transform.position.y, clampedZ);
    }
    //カメラ範囲を定義
    public void UpdateCameraBound(float right, float bottom)
    {
        _bounds = new CameraBounds(
            left : 0f,
            right : right,
            top : 0f,
            bottom : bottom
        );

        _isBoundSet = true;
    }

    //プレイヤをターゲットにする
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    //カメラ座標再設定
    public void SnapCameraPosition()
    {
        transform.position = target.position;
    }
}
