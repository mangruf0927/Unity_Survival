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
        Vector3 worldVec = transform.TransformDirection(moveVec);   // world space의 direction으로 변환

        rigid.linearVelocity = worldVec * moveSpeed;
    }

    // Input
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
    }
}
