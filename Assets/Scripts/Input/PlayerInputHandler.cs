using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;

    public void OnMove(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        playerController.SetDirection(direction);

        if (!playerController.IsGround()) return;

        if(direction == Vector2.zero) stateMachine.ChangeInputState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeInputState(PlayerStateEnums.MOVE);
    }

    public void OnRun(InputValue value)
    {
        playerController.SetRun(value.isPressed);
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed && playerController.IsGround()) 
        {
            stateMachine.ChangeInputState(PlayerStateEnums.JUMP);
        }
    }
}