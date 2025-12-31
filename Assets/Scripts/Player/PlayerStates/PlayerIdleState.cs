using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerController playerController;

    public PlayerIdleState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public void Enter()
    {
        Debug.Log("Idle 상태");
    }

    public void Update()
    {
        playerController.Look();
    }

    public void FixedUpdate()
    {
        
    }
    public void Exit()
    {
        
    }
}
