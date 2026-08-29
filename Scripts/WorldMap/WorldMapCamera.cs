using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    [SerializeField]  Camera mainCam;
    [SerializeField] Transform target;
    [SerializeField] float cameraSpeed = 5.0f;
    [SerializeField] Vector3 cameraOffset; 

    private void LateUpdate() 
    {
        if(target == null) { return; }

        Vector3 desiredPos = target.position + cameraOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, cameraSpeed * Time.deltaTime);
    }
}
