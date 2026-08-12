using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform groundCheck;

    private PlayerStats stats;
    private Rigidbody rigid;

    private Vector2 moveDirection;
    private Vector3 lookDirection = Vector3.forward;
    private bool isRun;

    public Rigidbody Rigid => rigid;
    public Vector2 Direction => moveDirection;
    public bool IsRun => isRun;

    private float riseMultiplier = 1.5f;
    private float fallMultiplier = 5f;

    private void Awake()
    {
        stats = GetComponentInChildren<PlayerStats>();
        rigid = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetRun(bool state) { isRun = state; stats.SetRun(state); }

    public Vector3 GetCameraDirection(Vector2 input)
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

    public bool IsGround()
    {
        return Physics.CheckSphere(groundCheck.position, 0.3f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
    }

    public void Move()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        float speed = isRun ? stats.RunSpeed : stats.MoveSpeed;

        Vector3 moveVec = GetCameraDirection(moveDirection);
        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }

    public void Look()
    {
        Vector3 lookVec = GetCameraDirection(moveDirection);
        if (lookVec.sqrMagnitude >= 0.0001f) lookDirection = lookVec.normalized;

        Quaternion target = Quaternion.LookRotation(lookDirection, Vector3.up);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target, Time.fixedDeltaTime * stats.RotateSpeed));
    }

    public void Stop()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, curVelocity.y, 0f);
    }

    public void Jump()
    {
        Vector3 velocity = rigid.linearVelocity;
        velocity.y = stats.JumpForce;
        rigid.linearVelocity = velocity;
    }

    public void Fall()
    {
        float multiplier = rigid.linearVelocity.y > 0f ? riseMultiplier : fallMultiplier;
        rigid.AddForce(Physics.gravity * (multiplier - 1f), ForceMode.Acceleration);
    }
}
