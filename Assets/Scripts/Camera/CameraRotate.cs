using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed;

    private bool isRightClick = false;
    private Vector2 mouseDelta;

    void LateUpdate()
    {
        Vector3 targetPos = target.position;
        
        if(isRightClick) transform.RotateAround(targetPos, Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime);        
        transform.LookAt(targetPos);

        // transform.RotateAround(targetPos, Vector3.right, -mouseDelta.y * rotateSpeed * Time.deltaTime);
    }

    public void SetCamAngle(Vector2 delta)
    {
        mouseDelta = delta;
    }

    public void SetRightClick(bool isClick)
    {
        isRightClick = isClick;
        if(!isClick) mouseDelta = Vector2.zero;
    }
}
