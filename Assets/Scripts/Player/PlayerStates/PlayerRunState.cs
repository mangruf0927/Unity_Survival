using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : IPlayerState
{
    private readonly PlayerController playerController;
    private readonly PlayerStateMachine stateMachine;

    public PlayerRunState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> InputHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.IDLE,
        PlayerStateEnums.MOVE,
        PlayerStateEnums.JUMP,
        PlayerStateEnums.ATTACK,
    };

    public HashSet<PlayerStateEnums> LogicHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public void Enter()
    {
        playerController.animator.SetFloat("speed", 2f);
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
    }
}