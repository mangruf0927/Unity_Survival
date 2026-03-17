using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private CameraRotate cameraRotate;
    [SerializeField] private Chest chest;

    private bool isRun;
    private bool isOpened;
    private Vector2 direction;

    private Outline currentOutline;
    private EquippableItem currentEquip;
    private Item currentItem;

    private void Update()
    {
        if (Camera.main == null || Mouse.current == null) return;

        UpdateTarget();

        int number = InputNumber();
        if (number != -1) playerController.EquipItem(number);

        if(isOpened) chest.Hold();
    }

    private void UpdateTarget()
    {
        EquippableItem nextEquip = null;
        Item nextItem = null;
        Outline nextOutline = null;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Equippable"))          
            {
                nextEquip = hit.collider.GetComponentInParent<EquippableItem>();
                if (nextEquip != null) nextOutline = hit.collider.GetComponentInParent<Outline>();
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Item"))   
            {
                nextItem = hit.collider.GetComponentInParent<Item>();
                if (nextItem != null) nextOutline = hit.collider.GetComponentInParent<Outline>();
            }
        }
        
        UpdateOutline(nextOutline);

        currentEquip = nextEquip;
        currentItem = nextItem;
    }

    private void ClearTarget()
    {
        if (currentOutline != null) currentOutline.enabled = false;
        
        currentOutline = null;
        currentEquip = null;
        currentItem = null;
    }

    private void UpdateOutline(Outline nextOutline)
    {
        if (nextOutline == currentOutline) return;

        if (currentOutline != null) currentOutline.enabled = false;

        currentOutline = nextOutline;

        if (currentOutline != null) currentOutline.enabled = true;
    }

    private int InputNumber()
    {
        for(int i = 0; i < 9; i++)
        {
            Key key = Key.Digit1 + i;
            Key numpadKey = Key.Numpad1 + i;

            if(Keyboard.current[key].wasPressedThisFrame || Keyboard.current[numpadKey].wasPressedThisFrame) return i;
        }
        return -1;
    }

    public void OnPick(InputValue value)        // E키 (무기 + 자루)
    {
        isOpened = value.isPressed;
        if(!isOpened)
        {
            chest.Cancel();
            return;
        }

        if (!value.isPressed || currentEquip == null) return;
        if (!playerController.GetEquippableItem(currentEquip)) return;

        ClearTarget();
    }

    public void OnCollect(InputValue value)     // F키 (자루 아이템)
    {
        if (!value.isPressed) return;

        if (currentItem != null)
        {
            if (playerController.GetCollectibleItem(currentItem))     
            {
                ClearTarget();
            }
        }
        else playerController.DropCollectibleItem();
    }

    public void OnDrop(InputValue value)
    {
        if (!value.isPressed) return;
        playerController.DropEquippableItem();
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
        if (!value.isPressed || playerController.CurrentWeapon == null) return;

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

    public void OnReload(InputValue value)
    {
        if(!value.isPressed || playerController.CurrentWeapon == null) return;

        if(playerController.CurrentWeapon is RangedWeapon rangedWeapon)
        {
            rangedWeapon.Reload();
        }
    }
}