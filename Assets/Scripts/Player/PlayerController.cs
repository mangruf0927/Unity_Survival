using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float moveSpeed = 3.0f;  
    [SerializeField] private float runSpeed = 5.0f;  

    private Vector2 moveDirection;
    private bool isRun;

    public void Move()
    {
        Vector3 moveVec = new(moveDirection.x, 0f, moveDirection.y);
        moveVec = Vector3.ClampMagnitude(moveVec, 1f);
        
        Vector3 temp = rigid.linearVelocity;
        float speed = isRun ? runSpeed : moveSpeed;
        rigid.linearVelocity = new Vector3(moveVec.x * speed, temp.y, moveVec.z * speed);
    }  

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetRun(bool state)
    {
        isRun = state;
    }

    public void StopPlayer()
    {
        Vector3 temp = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, temp.y, 0f);
    }
}
