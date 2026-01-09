using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStat playerStat;

    private Vector2 moveDirection;
    private bool isRun;
    private bool isGround;

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetRun(bool state) { isRun = state; }
    public bool IsGround() { return isGround;}
    public Vector2 GetDirection() { return moveDirection; }

    public void Move()
    {
        Vector3 moveVec = new(moveDirection.x, 0f, moveDirection.y);
        moveVec = Vector3.ClampMagnitude(moveVec, 1f);
        
        Vector3 temp = rigid.linearVelocity;
        float speed = isRun ? playerStat.runSpeed : playerStat.moveSpeed;
        rigid.linearVelocity = new Vector3(moveVec.x * speed, temp.y, moveVec.z * speed);
    }  

    public void Stop()
    {
        Vector3 temp = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, temp.y, 0f);
    }

    public void Jump()
    {
        isGround = false;
        rigid.AddForce(Vector3.up * playerStat.jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGround = true;
    }
}
