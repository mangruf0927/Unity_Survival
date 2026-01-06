using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    public PlayerMoveState( PlayerStateMachine _stateMachine, PlayerController _playerController)
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
        
    }

    public void FixedUpdate()
    {
        playerController.Move();
    }

    public void Exit()
    {
        
    }
}