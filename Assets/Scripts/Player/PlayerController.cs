using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform weaponPosition; 
    
    public Animator animator;
    public Weapon currentWeapon;

    private Vector2 moveDirection;
    private Vector3 lookDirection = Vector3.forward;

    private bool isRun;
    private bool isGround;

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetAimPoint(Vector3 point) { currentWeapon.SetAimPoint(point); }
    public void SetRun(bool state) { isRun = state; }
    
    public bool IsRun() { return isRun; }
    public bool IsGround() { return isGround;}
    public Vector2 GetDirection() { return moveDirection; }
    private Vector3 GetCameraDirection(Vector2 input)
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        return Vector3.ClampMagnitude(direction, 1f);
    }

    public void Move()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        float speed = isRun ? playerStat.runSpeed : playerStat.moveSpeed;

        Vector3 moveVec = GetCameraDirection(moveDirection);
        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }  

    public void Look()
    {
        Vector3 lookVec = GetCameraDirection(moveDirection);
        if (lookVec.sqrMagnitude >= 0.0001f) lookDirection = lookVec.normalized;

        Quaternion target = Quaternion.LookRotation(lookDirection, Vector3.up);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target,Time.fixedDeltaTime * playerStat.rotateSpeed));
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

    public void DropWeapon()
    {
        if(currentWeapon == null || !currentWeapon.canDrop) return;

        currentWeapon.ExitAttack();

        Weapon weapon = currentWeapon;
        currentWeapon = null;

        weapon.Drop();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGround = true;
    }
}
