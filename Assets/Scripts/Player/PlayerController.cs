using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] public float moveSpeed = 3.0f;
    
    private Vector2 moveDir;
    private Vector2 mouseDelta;

    public void SetDirection(Vector2 direction)
    {
        moveDir = direction;
    }

    public void SetMouseDelta(Vector2 delta)
    {
        mouseDelta = delta;
    }

    public void Move()
    {
        Vector3 moveVec = new Vector3(moveDir.x, 0f, moveDir.y);
        Vector3 worldVec = transform.TransformDirection(moveVec);   // world space의 direction으로 변환

        rigid.linearVelocity = worldVec * moveSpeed;
    }

    public void Look()
    {
        transform.Rotate(0f, mouseDelta.x, 0f);
    }
}