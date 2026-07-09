using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform equipPosition;
    [SerializeField] private InteractionUI interactionUI;
    [SerializeField] private ObjectPlacement objectPlacement;
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private EquippableSpawner equippableSpawner;

    private PlayerStats playerStats;
    private Rigidbody rigid;

    private Vector2 moveDirection;
    private Vector3 lookDirection = Vector3.forward;

    private bool isRun;
    private bool isGround;

    private EquippableItem currentEquipped;
    private Weapon currentWeapon;
    private Sack currentSack;
    private IInteractable currentInteractable;
    private RecoveryItem currentRecoveryItem;

    private bool isItemHovering;
    private bool isHolding;
    private float holdTimer;

    private bool isUsingRecovery;
    private float recoveryTimer;

    public Animator Animator => animator;
    public Weapon CurrentWeapon => currentWeapon;
    public RecoveryItem CurrentRecovery => currentRecoveryItem;

    public delegate void EquippedHandler(EquippableItem item);
    public event EquippedHandler OnEquipped;
    public event Action OnSackChanged;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        playerStats = GetComponentInChildren<PlayerStats>();
    }

    private void Update()
    {
        FindInteractable();
        UpdateHoldTimer();
        UpdateRecoveryTimer();
    }

    public void SetDirection(Vector2 direction) { moveDirection = direction; }
    public void SetRun(bool state) { isRun = state; }
    public void SetSack(Sack sack) { currentSack = sack; }
    public void SetRecoveryItem(RecoveryItem item) { currentRecoveryItem = item; }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        UpdateAmmo();
    }

    public void SetAimPoint(Vector3 point)
    {
        if (currentWeapon is RangedWeapon rangedWeapon)
            rangedWeapon.SetAimPoint(point);
    }

    public bool IsRun() { return isRun; }
    public bool IsGround() { return isGround; }

    public Vector2 GetDirection() { return moveDirection; }
    private Vector3 GetCameraDirection(Vector2 input)
    {
        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        return Vector3.ClampMagnitude(direction, 1f);
    }

    public void UpdateAnimation()
    {
        if (moveDirection == Vector2.zero) animator.SetFloat("speed", 0f);
        else if (IsRun()) animator.SetFloat("speed", 2f);
        else animator.SetFloat("speed", 1f);
    }

    public void Move()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        float speed = isRun ? playerStats.RunSpeed : playerStats.MoveSpeed;

        Vector3 moveVec = GetCameraDirection(moveDirection);
        rigid.linearVelocity = new Vector3(moveVec.x * speed, curVelocity.y, moveVec.z * speed);
    }

    public void Look()
    {
        Vector3 lookVec = GetCameraDirection(moveDirection);
        if (lookVec.sqrMagnitude >= 0.0001f) lookDirection = lookVec.normalized;

        Quaternion target = Quaternion.LookRotation(lookDirection, Vector3.up);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, target, Time.fixedDeltaTime * playerStats.RotateSpeed));
    }

    public void Stop()
    {
        Vector3 curVelocity = rigid.linearVelocity;
        rigid.linearVelocity = new Vector3(0f, curVelocity.y, 0f);
    }

    public void Jump()
    {
        isGround = false;
        rigid.AddForce(Vector3.up * playerStats.JumpForce, ForceMode.Impulse);
    }

    public void Eat(int hunger, int hp)
    {
        playerStats.EatFood(hunger, hp);
    }

    public void RecoverHp(int hp)
    {
        playerStats.RecoverHp(hp);
    }

    public void StartRecoveryUse()
    {
        if (currentRecoveryItem == null) return;

        isUsingRecovery = true;
    }

    public void CancelRecoveryUse()
    {
        isUsingRecovery = false;
        recoveryTimer = 0f;
    }

    public void StartPlacement(PlaceableItem item)
    {
        objectPlacement.StartPlacement(item, this);
    }

    public void CancelPlacement()
    {
        objectPlacement.CancelPlacement();
    }

    public bool GetEquippableItem(EquippableItem item)
    {
        if (item == null) return false;
        if (!inventory.AddItem(item, out EquippableItem prevItem)) return false;

        item.UnregisterEquippable();

        item.Attach(equipPosition);
        item.gameObject.SetActive(false);

        if (ReplacedItem(prevItem, item))
        {
            currentEquipped = item;
            item.OnEquip(this);
            UpdateUpperBodyWeight();
        }
        return true;
    }

    public InventorySaveData CreateInventorySaveData()
    {
        return inventory.CreateSaveData(currentEquipped);
    }

    public void LoadInventorySaveData(InventorySaveData data, EquippableDatabase equippableDatabase, ItemDataBase itemDatabase)
    {
        if (data == null || equippableDatabase == null || itemDatabase == null) return;

        if (currentEquipped != null)
        {
            currentEquipped.OnUnequip(this);
        }

        currentEquipped = null;
        currentWeapon = null;
        currentSack = null;

        inventory.LoadSaveData(data, equippableDatabase, itemDatabase);

        foreach (EquippableItem item in inventory.ItemList)
        {
            if (item == null) continue;

            item.Attach(equipPosition);
            item.gameObject.SetActive(false);
        }

        if (data.equippedIndex >= 0)
        {
            EquipItem(data.equippedIndex);
            return;
        }

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    private bool ReplacedItem(EquippableItem prevItem, EquippableItem newItem)
    {
        if (prevItem == null) return false;

        bool equipped = currentEquipped == prevItem;
        if (equipped)
        {
            prevItem.OnUnequip(this);
            currentEquipped = null;
            currentWeapon = null;
            currentSack = null;
            currentRecoveryItem = null;
        }

        if (prevItem is Sack prevSack && newItem is Sack newSack)
        {
            prevSack.MoveItems(newSack);
            newSack.GetComponentInChildren<SackItemCount>(true).gameObject.SetActive(true);
        }
        Destroy(prevItem.gameObject);
        return equipped;
    }

    public void DropEquippableItem()
    {
        if (currentEquipped == null || !currentEquipped.CanDrop) return;

        EquippableItem item = currentEquipped;

        item.OnUnequip(this);
        inventory.RemoveItem(item);

        item.gameObject.SetActive(true);
        item.Detach();

        if (equippableSpawner != null)
        {
            equippableSpawner.Register(item);
        }

        currentEquipped = null;
        currentWeapon = null;
        currentSack = null;
        currentRecoveryItem = null;

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    public void ConsumeEquippedItem(EquippableItem item)
    {
        if (item == null) return;
        if (currentEquipped != item) return;

        item.OnUnequip(this);
        inventory.RemoveItem(item);

        currentEquipped = null;
        currentWeapon = null;
        currentSack = null;
        currentRecoveryItem = null;

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);

        Destroy(item.gameObject);
    }

    public void EquipItem(int idx)
    {
        EquippableItem nextItem = inventory.SelectItem(idx);
        if (nextItem == null) return;

        if (currentEquipped == nextItem)
        {
            UnequipItem();
            return;
        }

        if (currentEquipped != null) currentEquipped.OnUnequip(this);

        currentEquipped = nextItem;
        currentEquipped.OnEquip(this);

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    public void UnequipItem()
    {
        if (currentEquipped == null) return;

        currentEquipped.OnUnequip(this);
        currentEquipped = null;
        currentSack = null;
        currentWeapon = null;
        currentRecoveryItem = null;

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    public bool GetCollectibleItem(Item item)
    {
        if (currentSack == null) return false;

        bool added = currentSack.AddItem(item);

        if (added)
        {
            OnSackChanged?.Invoke();
        }

        return added;
    }

    public void DropCollectibleItem()
    {
        if (currentSack == null) return;

        Item item = currentSack.DropItem();
        if (item == null) return;

        item.transform.position = transform.position + transform.forward * 1.5f + Vector3.up;
        item.ResetPhysics();

        if (itemSpawner != null) itemSpawner.Register(item);

        OnSackChanged?.Invoke();
    }

    public void AddAmmo(AmmoType ammoType, int amount)
    {
        inventory.AddAmmo(ammoType, amount);
        UpdateAmmo();
    }

    public void Reload()
    {
        if (currentWeapon is RangedWeapon rangedWeapon)
        {
            int amount = inventory.UseAmmo(rangedWeapon.Type, rangedWeapon.NeedAmmo());
            rangedWeapon.Reload(amount);
            UpdateAmmo();
        }
    }

    private void UpdateAmmo()
    {
        if (currentWeapon is RangedWeapon rangedWeapon)
        {
            rangedWeapon.SetTotalAmmo(inventory.GetAmmoCount(rangedWeapon.Type));
        }
    }

    public void SetItemHovering(bool state)
    {
        isItemHovering = state;

        if (isItemHovering)
        {
            holdTimer = 0f;
            isHolding = false;
            if (interactionUI != null) interactionUI.Hide();
        }
    }

    private void FindInteractable()
    {
        IInteractable prev = currentInteractable;
        currentInteractable = null;


        Collider[] hitArray = Physics.OverlapSphere(transform.position, playerStats.InteractDistance, ~0, QueryTriggerInteraction.Collide);

        float closestDistance = float.MaxValue;
        foreach (Collider hit in hitArray)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable == null || !interactable.CanInteract(this)) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }

        if (prev != currentInteractable) holdTimer = 0f;

        if (interactionUI == null) return;

        if (currentInteractable != null && !isItemHovering) interactionUI.Show(currentInteractable.UIPosition);
        else interactionUI.Hide();
    }

    private void UpdateHoldTimer()
    {
        if (!isHolding || currentInteractable == null)
        {
            interactionUI.SetProgress(0f);
            return;
        }

        holdTimer += Time.deltaTime;

        float progress = holdTimer / currentInteractable.HoldTime;
        interactionUI.SetProgress(progress);

        if (holdTimer >= currentInteractable.HoldTime)
        {
            currentInteractable.Interact(this);

            holdTimer = 0f;
            isHolding = false;
            interactionUI.SetProgress(0f);
        }
    }

    private void UpdateRecoveryTimer()
    {
        if (!isUsingRecovery || currentRecoveryItem == null) return;

        recoveryTimer += Time.deltaTime;

        if (recoveryTimer < currentRecoveryItem.HoldTime) return;

        RecoveryItem item = currentRecoveryItem;
        CancelRecoveryUse();
        item.Apply(this);
    }

    public bool HasInteractable()
    {
        return currentInteractable != null;
    }

    public void SetHolding(bool state)
    {
        isHolding = state;
        if (!isHolding)
        {
            holdTimer = 0f;
            interactionUI.SetProgress(0f);
        }
    }

    private void UpdateUpperBodyWeight()
    {
        animator.SetLayerWeight(1, currentEquipped == null ? 0f : 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGround = true;
    }
}
