using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerIdleState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> inputHash { get; } = new HashSet<PlayerStateEnums>()
    {
        PlayerStateEnums.MOVE,
        PlayerStateEnums.RUN,
        PlayerStateEnums.JUMP,
    };

    public HashSet<PlayerStateEnums> logicHash { get; } = new HashSet<PlayerStateEnums>()
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
