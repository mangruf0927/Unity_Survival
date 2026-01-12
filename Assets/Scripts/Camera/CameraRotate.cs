using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float distance;

    private bool isRightClick = false;
    private Vector2 mouseDelta;

    void LateUpdate()
    {
        Vector3 targetPos = target.position;

        if (isRightClick) transform.RotateAround(targetPos, Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime);

        Vector3 direction = transform.position - targetPos;
        direction.y = 0f;
        direction.Normalize();

        Vector3 pos = targetPos + direction * distance;
        pos.y = targetPos.y + 2;
        
        transform.position = pos;

        mouseDelta = Vector2.zero;
    }
    
    // transform.RotateAround(targetPos, Vector3.right, -mouseDelta.y * rotateSpeed * Time.deltaTime);

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
