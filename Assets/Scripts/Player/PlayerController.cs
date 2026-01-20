using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] public Animator animator;

    private Vector2 moveDirection;
    private bool isRun;
    private bool isGround;

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetRun(bool state) { isRun = state; }
    public bool IsRun() { return isRun; }
    public bool IsGround() { return isGround;}
    public Vector2 GetDirection() { return moveDirection; }

    public void Move()
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveVec = right * moveDirection.x + forward * moveDirection.y;
        moveVec = Vector3.ClampMagnitude(moveVec, 1f);

        Vector3 curVelocity = rigid.linearVelocity;
        float speed = isRun ? playerStat.runSpeed : playerStat.moveSpeed;

        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }  

    public void Look()
    {
        Quaternion target = Quaternion.Euler(0f, cameraPivot.eulerAngles.y, 0f);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target, Time.fixedDeltaTime * playerStat.rotateSpeed));
    }

    public void Stop()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, curVelocity.y, 0f);
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
