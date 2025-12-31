using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    private PlayerStateMachine stateMachine;
    private PlayerController playerController;

    public PlayerMoveState(PlayerStateMachine _stateMachine, PlayerController _playerController)
    {
        stateMachine = _stateMachine;
        playerController = _playerController;
    }

    public void Enter()
    {
        Debug.Log("Move 상태");
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
        
    }
}