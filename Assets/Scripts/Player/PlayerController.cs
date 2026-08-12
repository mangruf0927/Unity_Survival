using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;


    [SerializeField] private Animator animator;
    [SerializeField] private InventoryProvider inventoryProvider;
    [SerializeField] private Transform equipPosition;
    [SerializeField] private InteractionUI interactionUI;
    [SerializeField] private ObjectPlacement objectPlacement;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EquippableRegistry equippableRegistry;

    [SerializeField] private LayerMask interactableLayerMask;

    private PlayerStats playerStats;

    private EquippableItem currentEquipped;
    private Weapon currentWeapon;
    private Sack currentSack;
    private IInteractable currentInteractable;
    private RecoveryItem currentRecoveryItem;
    private Inventory inventory;

    private bool isItemHovering;
    private bool isHolding;
    private float holdTimer;

    private bool isUsingRecovery;
    private float recoveryTimer;

    public Rigidbody Rigid => movement.Rigid;
    public Animator Animator => animator;
    public Weapon CurrentWeapon => currentWeapon;
    public RecoveryItem CurrentRecovery => currentRecoveryItem;

    public delegate void EquippedHandler(EquippableItem item);
    public event EquippedHandler OnEquipped;
    public event Action OnSackChanged;

    private void Awake()
    {
        inventory = inventoryProvider.Inventory;
        playerStats = GetComponentInChildren<PlayerStats>();
    }

    private void Update()
    {
        FindInteractable();
        UpdateHoldTimer();
        UpdateRecoveryTimer();
    }

    public void SetDirection(Vector2 direction) => movement.SetDirection(direction);
    public Vector2 GetDirection() => movement.Direction;

    public bool IsGround() => movement.IsGround();

    public void SetRun(bool state) => movement.SetRun(state);
    public bool IsRun() => movement.IsRun;

    public void Move() => movement.Move();
    public void Look() => movement.Look();
    public void Stop() => movement.Stop();
    public void Jump() => movement.Jump();
    public void Fall() => movement.Fall();

    public void UpdateAnimation()
    {
        if (GetDirection() == Vector2.zero) animator.SetFloat("speed", 0f);
        else if (IsRun()) animator.SetFloat("speed", 2f);
        else animator.SetFloat("speed", 1f);
    }

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
        if (!inventory.AddItem(item, out EquippableItem prevItem, out bool isStacked)) return false;

        item.UnregisterEquippable();

        if (isStacked)
        {
            // pool로 수정
            Destroy(item.gameObject);
            return true;
        }

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
        int itemCount = inventory.GetItemCount(item);

        if (itemCount <= 0) return;

        if (itemCount > 1)
        {
            if (!inventory.RemoveItem(item, out _)) return;

            EquippableItem droppedItem = Instantiate(item, item.transform.position, item.transform.rotation);
            droppedItem.Detach();
            droppedItem.gameObject.SetActive(true);

            equippableRegistry.Register(droppedItem);
            return;
        }

        if (!inventory.RemoveItem(item, out bool isEmpty) || !isEmpty) return;

        item.OnUnequip(this);
        item.Detach();
        item.gameObject.SetActive(true);

        equippableRegistry.Register(item);

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
        if (!inventory.RemoveItem(item, out bool isEmpty)) return;

        if (!isEmpty) return;

        item.OnUnequip(this);

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

        if (itemRegistry != null) itemRegistry.Register(item);

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

    private readonly Collider[] hitBuffer = new Collider[32];

    private void FindInteractable()
    {
        IInteractable previousInteractable = currentInteractable;
        currentInteractable = null;

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, playerStats.InteractDistance, hitBuffer,
            interactableLayerMask, QueryTriggerInteraction.Collide);

        Vector3 playerPosition = transform.position;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = hitBuffer[i];

            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null || !interactable.CanInteract(this)) continue;

            float distance = (hit.transform.position - playerPosition).sqrMagnitude;
            if (distance >= closestDistance) continue;

            closestDistance = distance;
            currentInteractable = interactable;
        }
        if (previousInteractable != currentInteractable) holdTimer = 0f;

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

    // Save / Load
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

        foreach (InventoryItem inventoryItem in inventory.ItemList)
        {
            EquippableItem item = inventoryItem.Item;

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
}
