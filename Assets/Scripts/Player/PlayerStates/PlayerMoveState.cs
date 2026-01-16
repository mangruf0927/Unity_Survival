using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerMoveState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> inputHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.IDLE,
        PlayerStateEnums.JUMP,
    };

    public HashSet<PlayerStateEnums> logicHash { get; } = new HashSet<PlayerStateEnums>()
    {
        
    };

    public void Enter()
    {
        playerController.SetAnimation("isWalk", true);
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        playerController.Move();
        playerController.Look();
    }

    public void Exit()
    {
        playerController.SetAnimation("isWalk", false);
    }
}