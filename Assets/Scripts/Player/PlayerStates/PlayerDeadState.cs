using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : IPlayerState
{
    private readonly PlayerController playerController;
    private readonly PlayerStateMachine stateMachine;

    public PlayerDeadState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> InputHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public HashSet<PlayerStateEnums> LogicHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public void Enter()
    {
        playerController.animator.SetBool("IsDead", true);
    }

    public void Update()
    {
        
    }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }
}
