using UnityEngine;

public class PlayerStat : HealthBase
{
    [SerializeField] PlayerStateMachine playerStateMachine;

    [Header("이동 속도")]
    public float moveSpeed;  

    [Header("달리기 속도")]
    public float runSpeed;  

    [Header("점프 높이")]
    public float jumpForce;

    [Header("회전 속도")]
    public float rotateSpeed;

    [Header("체력")]
    [SerializeField] private int maxHP = 100;
    protected override int MaxHP => maxHP;

    protected override void Die()
    {
        playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }
}
