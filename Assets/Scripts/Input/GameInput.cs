using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class GameInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private CameraRotate cameraRotate;

    private bool isRun;
    private Vector2 direction;

    private Outline currentOutline;
    private EquippableItem currentEquipped;
    private Item currentItem;
    private Camera mainCamera;

    public event Action<Item> OnHoverItem;
    public event Action OnExitItem;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null || Mouse.current == null) return;
        UpdateTarget();

        int number = InputNumber();
        if (number != -1) playerController.EquipItem(number);

        OnAttack();
    }

    private void OnAttack()
    {
        if (playerController.CurrentWeapon == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject()) return;

        Ray camRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(camRay, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Collide)) playerController.SetAimPoint(hit.point);
        else playerController.SetAimPoint(camRay.origin + camRay.direction * 1000f);

        stateMachine.ChangeInputState(PlayerStateEnums.ATTACK);
    }

    private void UpdateTarget()
    {
        EquippableItem nextEquip = null;
        Item nextItem = null;
        Outline nextOutline = null;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 15f, ~0, QueryTriggerInteraction.Collide))
        {
            nextEquip = hit.collider.GetComponentInParent<EquippableItem>();
            nextItem = hit.collider.GetComponentInParent<Item>();

            if (nextEquip != null && nextEquip.IsAttached)
            {
                nextEquip = null;
            }
            else if (nextEquip != null)
            {
                nextOutline = hit.collider.GetComponentInParent<Outline>();
            }
            else if (nextItem != null)
            {
                nextOutline = hit.collider.GetComponentInParent<Outline>();
            }
        }

        UpdateOutline(nextOutline);

        if (currentItem != nextItem)
        {
            if (nextItem != null) OnHoverItem?.Invoke(nextItem);
            else OnExitItem?.Invoke();
        }

        currentEquipped = nextEquip;
        currentItem = nextItem;

        playerController.SetItemHovering(currentItem != null || currentEquipped != null);
    }

    private void ClearTarget()
    {
        if (currentOutline != null) currentOutline.enabled = false;

        currentOutline = null;
        currentEquipped = null;
        currentItem = null;

        OnExitItem?.Invoke();
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
        for (int i = 0; i < 9; i++)
        {
            Key key = Key.Digit1 + i;
            Key numpadKey = Key.Numpad1 + i;

            if (Keyboard.current[key].wasPressedThisFrame || Keyboard.current[numpadKey].wasPressedThisFrame) return i;
        }
        return -1;
    }

    public void OnPick(InputValue value)        // E키 (총알 + 음식 + 장비 + 상호작용)
    {
        if (!value.isPressed)
        {
            playerController.SetHolding(false);
            return;
        }

        if (currentItem != null && currentItem.Data.ItemType == ItemType.AMMO)
        {
            playerController.AddAmmo(currentItem.Data.AmmoData.AmmoType, currentItem.Data.AmmoData.Amount);
            Destroy(currentItem.gameObject);
            ClearTarget();
            return;
        }
        else if (currentItem != null && currentItem.Data.ItemType == ItemType.FOOD)
        {
            playerController.Eat(currentItem.Data.FoodData.HungerAmount, currentItem.Data.FoodData.HpAmount);
            Destroy(currentItem.gameObject);
            ClearTarget();
            return;
        }

        if (currentEquipped != null)
        {
            if (!playerController.GetEquippableItem(currentEquipped)) return;
            ClearTarget();
            return;
        }

        if (playerController.HasInteractable())
        {
            playerController.SetHolding(true);
            return;
        }
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

        if (direction == Vector2.zero) stateMachine.ChangeInputState(PlayerStateEnums.IDLE);
        else stateMachine.ChangeInputState(isRun ? PlayerStateEnums.RUN : PlayerStateEnums.MOVE);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && playerController.IsGround())
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

    public void OnZoom(InputValue value)
    {
        float y = value.Get<Vector2>().y;
        cameraRotate.SetZoomY(y);
    }

    public void OnReload(InputValue value)
    {
        if (!value.isPressed || playerController.CurrentWeapon == null) return;
        playerController.Reload();
    }
}
