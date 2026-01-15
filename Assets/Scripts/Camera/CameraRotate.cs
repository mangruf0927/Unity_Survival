using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float distance = 5f; 

    private bool isRightClick = false;
    private Vector2 mouseDelta;

    private float x = 0f;
    private float y = 0f;

    void LateUpdate()
    {
        Vector3 targetPos = target.position;

        if (isRightClick) UpdateAngles();

        Quaternion rot = Quaternion.Euler(y, x, 0f);
        Vector3 pos = targetPos + rot * new Vector3(0f, 0f, -distance);

        transform.SetPositionAndRotation(pos, rot);

        mouseDelta = Vector2.zero;
    }

    private void UpdateAngles()
    {
        x += mouseDelta.x * rotateSpeed * Time.deltaTime;

        float deltaY = -mouseDelta.y * rotateSpeed * Time.deltaTime;
        bool isLimit = (y >= 89f && deltaY > 0f) || (y <= -89f && deltaY < 0f);
            
        if(!isLimit) y = Mathf.Clamp(y + deltaY, -89f, 89f);
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
