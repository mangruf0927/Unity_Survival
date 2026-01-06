using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float moveSpeed = 3.0f;  

    private Vector2 moveDirection;

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector3 moveVec = new(moveDirection.x, 0f, moveDirection.y);
        moveVec = Vector3.ClampMagnitude(moveVec, 1f);
        
        Vector3 temp = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(moveVec.x * moveSpeed, temp.y, moveVec.z * moveSpeed);
    }  

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }
}
