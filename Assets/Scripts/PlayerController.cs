using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private Rigidbody rigid;
    private Vector2 moveDir;

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector3 moveVec = new Vector3(moveDir.x, 0f, moveDir.y);
        rigid.linearVelocity = moveVec * moveSpeed;
    }

    // Input
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }
}