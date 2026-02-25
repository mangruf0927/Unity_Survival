using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private CameraRotate cameraRotate;

    private bool isRun;
    private Vector2 direction;

    private Outline currentOutline;
    void Update()
    {
        if (Camera.main == null || Mouse.current == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        Outline nextOutline = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Collide))
        {
            // Debug.Log(hit.collider.gameObject.layer);

            if (hit.collider.gameObject.layer == 9)
            {
                if (nextOutline == null) nextOutline = hit.collider.GetComponent<Outline>();
            }
        }

        if (nextOutline != currentOutline)
        {
            if (currentOutline != null) currentOutline.enabled = false;
            currentOutline = nextOutline;
            if (currentOutline != null) currentOutline.enabled = true;
        }
    }

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
        if (!value.isPressed || playerController.currentWeapon == null) return;

        Ray camRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(camRay, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Collide)) playerController.SetAimPoint(hit.point);
        else playerController.SetAimPoint(camRay.origin + camRay.direction * 1000f);

        stateMachine.ChangeInputState(PlayerStateEnums.ATTACK);
    }

    public void OnZoom(InputValue value)
    {
        float y = value.Get<Vector2>().y;
        cameraRotate.SetZoomY(y);
    }

    public void OnDrop(InputValue value)
    {
        playerController.DropWeapon();
    }
}