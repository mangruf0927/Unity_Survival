using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState :  IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerJumpState(PlayerStateMachine _stateMachine, PlayerController _playerController)
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
        playerController.Jump();
        playerController.SetAnimation("isJump", true);
    }

    public void Update()
    {
        if(!playerController.IsGround()) return;

        if(playerController.GetDirection() == Vector2.zero)
            stateMachine.ChangeLogicState(PlayerStateEnums.IDLE);
        else
            stateMachine.ChangeLogicState(playerController.IsRun() ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);
    }

    public void FixedUpdate()
    {
        playerController.Move();
    }

    public void Exit()
    {
        playerController.SetAnimation("isJump", false);
    }
}