using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerController playerController;

    public PlayerRunState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public void Enter()
    {
        Debug.Log("Run 상태");
        playerController.moveSpeed *= 2;
    }

    public void Update()
    {
        playerController.Look();
    }

    public void FixedUpdate()
    {
        playerController.Move();
    }

    public void Exit()
    {
        playerController.moveSpeed /= 2;
    }
}