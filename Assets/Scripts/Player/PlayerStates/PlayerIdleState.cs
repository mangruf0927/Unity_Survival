using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    private readonly PlayerController playerController;
    private readonly PlayerStateMachine stateMachine;

    public PlayerIdleState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> InputHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.MOVE,
        PlayerStateEnums.RUN,
        PlayerStateEnums.JUMP,
        PlayerStateEnums.ATTACK,
    };

    public HashSet<PlayerStateEnums> LogicHash { get; } = new HashSet<PlayerStateEnums>()
    {
        
    };

    public void Enter()
    {
        playerController.Stop();
        playerController.animator.SetFloat("speed", 0f);
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
