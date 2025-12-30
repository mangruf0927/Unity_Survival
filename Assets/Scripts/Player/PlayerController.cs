using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Rigidbody rigid;
    
    private Vector2 moveDir;
    private Vector2 mouseDelta;
    private bool isRotating = false;
    private Vector2 fixedCursorPos;

    public void Move()
    {
        Vector3 moveVec = new Vector3(moveDir.x, 0f, moveDir.y);
        Vector3 worldVec = transform.TransformDirection(moveVec);   // world space의 direction으로 변환

        rigid.linearVelocity = worldVec * moveSpeed;
    }

    public void Look()
    {
        if(isRotating) 
        {
            float mouseX = mouseDelta.x;
            transform.Rotate(0f, mouseX, 0f);

            Mouse.current.WarpCursorPosition(fixedCursorPos);
        }
    }

    // Input
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();
    }

    public void OnMouse(InputValue value)
    {
        if(value.isPressed && !isRotating)
        {
            fixedCursorPos = Mouse.current.position.ReadValue();
        }

        isRotating = value.isPressed;
    }
}