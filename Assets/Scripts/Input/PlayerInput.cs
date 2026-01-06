using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;

    private Vector2 direction;

    public void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
        playerController.SetDirection(direction);

        if(direction == Vector2.zero) stateMachine.ChangeState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeState(PlayerStateEnums.MOVE);
    }
}