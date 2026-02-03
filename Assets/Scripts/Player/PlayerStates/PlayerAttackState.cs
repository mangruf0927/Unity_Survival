using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerAttackState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> inputHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public HashSet<PlayerStateEnums> logicHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.IDLE,
        PlayerStateEnums.MOVE,
        PlayerStateEnums.RUN,
    };

    public void Enter()
    {
        playerController.animator.SetBool("isAttack", true);
        // playerController.meleeWeapon.BeginAttack();
        playerController.gunWeapon.Shoot();
    }

    public void Update()
    {
        var state = playerController.animator.GetCurrentAnimatorStateInfo(0);

        if(state.IsName("attack") && state.normalizedTime >= 1.0f)
        {
            if(playerController.GetDirection() == Vector2.zero)
                stateMachine.ChangeLogicState(PlayerStateEnums.IDLE);
            else
                stateMachine.ChangeLogicState(playerController.IsRun() ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);
        }
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        playerController.animator.SetBool("isAttack", false);
        // playerController.meleeWeapon.EndAttack();
    }
}
