using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float zoomSpeed;

    [SerializeField] private float distance; 
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    private Vector2 mouseDelta;
    private bool isRightClick = false;
    private float scrollY = 0f;

    private float yaw;          // 좌우     
    private float pitch;        // 상하

    void LateUpdate()
    {
        Zoom();
        UpdateAngles();
        ApplyCamera();
    }

    private void UpdateAngles()
    {
        if (!isRightClick) return;

        yaw += mouseDelta.x * rotateSpeed * Time.deltaTime;

        float deltaY = -mouseDelta.y * rotateSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch + deltaY, -89f, 89f);
    }

    private void ApplyCamera()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 position = target.position + rotation * new Vector3(0f, 0f, -distance);

        transform.SetPositionAndRotation(position, rotation);

        mouseDelta = Vector2.zero;
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

    public void SetZoomY(float y)
    {
        scrollY += y;
    }

    public void Zoom()
    {
        scrollY = Mathf.Lerp(scrollY, 0f, 4f * Time.deltaTime);
        float delta = -scrollY * zoomSpeed * 0.01f;
        distance = Mathf.Clamp(distance + delta, minDistance, maxDistance);
    }
}
