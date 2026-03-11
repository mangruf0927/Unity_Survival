using UnityEngine;

public class PlayerStat : HealthBase
{
    [SerializeField] PlayerStateMachine playerStateMachine;
    
    [SerializeField] private int maxHP = 100;
    public override int MaxHP => maxHP;

    public float moveSpeed;  
    public float runSpeed;  
    public float jumpForce;
    public float rotateSpeed;
    
    protected override void Die()
    {
        playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }
}
