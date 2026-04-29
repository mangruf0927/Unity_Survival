using UnityEngine;

public class PlayerStats : HealthBase
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private PlayerDataSO playerData;

    public override int MaxHP => playerData.maxHP;
    public float MoveSpeed => playerData.moveSpeed;
    public float RunSpeed => playerData.runSpeed;
    public float JumpForce => playerData.jumpForce;
    public float RotateSpeed => playerData.rotateSpeed;

    protected override void Die()
    {
        playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }
}
