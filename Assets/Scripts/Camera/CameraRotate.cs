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
    private Vector3 cameraPos = new(0, 0, -1);

    void LateUpdate()
    {
        Vector3 targetPos = target.position;

        if (isRightClick) 
        {
            transform.RotateAround(targetPos, Vector3.up, mouseDelta.x * rotateSpeed * Time.deltaTime);
            transform.RotateAround(targetPos, Vector3.right, -mouseDelta.y * rotateSpeed * Time.deltaTime);

            cameraPos = (transform.position - targetPos).normalized;
        }
        Vector3 pos = targetPos + cameraPos * distance;
        Quaternion rot = Quaternion.LookRotation((targetPos - pos).normalized, Vector3.up);

        Vector3 euler = rot.eulerAngles;
        if (euler.x > 180f) euler.x -= 360f;
        euler.x = Mathf.Clamp(euler.x, -80f, 80f);
        
        rot = Quaternion.Euler(euler.x, euler.y, 0f);

        transform.SetPositionAndRotation(pos, rot);
        
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
}
