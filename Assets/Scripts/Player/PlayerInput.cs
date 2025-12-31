using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;

    private bool isRotating;
    private Vector2 fixedCursorPos;

    void LateUpdate()
    {
        if (isRotating) Mouse.current.WarpCursorPosition(fixedCursorPos);
    }

    public void OnMove(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();

        if(direction == Vector2.zero) stateMachine.ChangeState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeState(PlayerStateEnums.MOVE);

        playerController.SetDirection(direction);
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
}
