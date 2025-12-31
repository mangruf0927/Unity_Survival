using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;

    private bool isRotating;
    private bool isRunPressed;
    private Vector2 fixedCursorPos;
    private Vector2 direction;

    void LateUpdate()
    {
        if (isRotating) Mouse.current.WarpCursorPosition(fixedCursorPos);
    }

    public void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
        playerController.SetDirection(direction);

        if(direction == Vector2.zero) stateMachine.ChangeState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeState(isRunPressed ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);

    }

    public void OnLook(InputValue value)
    {
        Vector2 delta = value.Get<Vector2>();
        
        if (isRotating) playerController.SetMouseDelta(delta);
        else playerController.SetMouseDelta(Vector2.zero);
    }

    public void OnMouse(InputValue value)
    {
        if(value.isPressed && !isRotating) fixedCursorPos = Mouse.current.position.ReadValue();
        
        isRotating = value.isPressed;
    }

    public void OnRun(InputValue value)
    {
        isRunPressed = value.isPressed;

        if (direction != Vector2.zero)
        stateMachine.ChangeState(isRunPressed ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);

    }
}
