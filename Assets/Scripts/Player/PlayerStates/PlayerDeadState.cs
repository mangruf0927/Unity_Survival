using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerDeadState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public HashSet<PlayerStateEnums> inputHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public HashSet<PlayerStateEnums> logicHash { get; } = new HashSet<PlayerStateEnums>()
    {
    };

    public void Enter()
    {
        playerController.animator.SetTrigger("OnDead");
        Debug.Log("죽음 ㅜㅜ");
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
