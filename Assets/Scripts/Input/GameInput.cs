using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private CameraRotate cameraRotate;

    private bool isRun;
    private Vector2 direction;

    public void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
        playerController.SetDirection(direction);

        UpdateMoveState();
    }

    public void OnRun(InputValue value)
    {
        isRun = value.isPressed;
        playerController.SetRun(isRun);

        UpdateMoveState();
    }

    private void UpdateMoveState()
    {
        if (!playerController.IsGround()) return;

        if(direction == Vector2.zero) stateMachine.ChangeInputState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeInputState(isRun ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed && playerController.IsGround()) 
        {
            stateMachine.ChangeInputState(PlayerStateEnums.JUMP);
        }
    }

    public void OnLook(InputValue value)
    {
        cameraRotate.SetCamAngle(value.Get<Vector2>());
    }

    public void OnRotate(InputValue value)
    {
        cameraRotate.SetRightClick(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        Ray camRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(camRay, out RaycastHit hit)) playerController.SetAimPoint(hit.point);
        else playerController.SetAimPoint(camRay.origin + camRay.direction * 1000f);

        stateMachine.ChangeInputState(PlayerStateEnums.ATTACK);
    }

    public void OnZoom(InputValue value)
    {
        float y = value.Get<Vector2>().y;
        cameraRotate.SetZoomY(y);
    }
}