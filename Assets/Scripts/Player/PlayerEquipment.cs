using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private Transform equipPosition;
    [SerializeField] private InventoryProvider inventoryProvider;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EquippableRegistry equippableRegistry;

    private Animator animator;
    private PlayerController playerController;
    private Inventory inventory;

    private EquippableItem currentEquipped;
    private Weapon currentWeapon;
    private Sack currentSack;
    private RecoveryItem currentRecovery;

    private bool isUsingRecovery;
    private float recoveryTimer;

    public Weapon CurrentWeapon => currentWeapon;
    public RecoveryItem CurrentRecovery => currentRecovery;

    public event Action<EquippableItem> OnEquipped;
    public event Action OnSackChanged;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = playerController.Animator;
    }

    private void Start()
    {
        inventory = inventoryProvider.Inventory;
    }

    private void Update()
    {
        UpdateRecoveryTimer();
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        UpdateAmmo();
    }

    public void SetSack(Sack sack)
    {
        currentSack = sack;
    }

    public void SetRecoveryItem(RecoveryItem recovery)
    {
        currentRecovery = recovery;
    }

    public void SetAimPoint(Vector3 point)
    {
        if (currentWeapon is RangedWeapon ranged)
            ranged.SetAimPoint(point);
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
            item.OnEquip(playerController);
            UpdateUpperBodyWeight();
            OnEquipped?.Invoke(currentEquipped);
        }
        return true;
    }

    private bool ReplacedItem(EquippableItem prevItem, EquippableItem newItem)
    {
        if (prevItem == null || newItem == null) return false;

        bool equipped = currentEquipped == prevItem;
        if (equipped)
        {
            prevItem.OnUnequip(playerController);
            ClearEquipment();
        }

        if (prevItem is Sack prevSack && newItem is Sack newSack)
        {
            prevSack.MoveItems(newSack);
            newSack.GetComponentInChildren<SackItemCount>(true).gameObject.SetActive(true);
        }
        Destroy(prevItem.gameObject);
        return equipped;
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

        if (currentEquipped != null) currentEquipped.OnUnequip(playerController);

        currentEquipped = nextItem;
        currentEquipped.OnEquip(playerController);

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    public void UnequipItem()
    {
        if (currentEquipped == null) return;
        currentEquipped.OnUnequip(playerController);

        ClearEquipment();
        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
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

        item.OnUnequip(playerController);
        item.Detach();
        item.gameObject.SetActive(true);

        equippableRegistry.Register(item);

        ClearEquipment();
        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }

    public void ConsumeEquippedItem(EquippableItem item)
    {
        if (item == null || currentEquipped != item) return;

        if (!inventory.RemoveItem(item, out bool isEmpty)) return;
        if (!isEmpty) return;

        item.OnUnequip(playerController);

        ClearEquipment();
        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);

        Destroy(item.gameObject);
    }

    private void ClearEquipment()
    {
        currentEquipped = null;
        currentWeapon = null;
        currentSack = null;
        currentRecovery = null;

        isUsingRecovery = false;
        recoveryTimer = 0f;
    }

    public bool GetCollectibleItem(Item item)
    {
        if (currentSack == null) return false;

        bool added = currentSack.AddItem(item);

        if (added) OnSackChanged?.Invoke();

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

    public void StartRecoveryUse()
    {
        if (currentRecovery == null) return;
        isUsingRecovery = true;
    }

    public void CancelRecoveryUse()
    {
        isUsingRecovery = false;
        recoveryTimer = 0f;
    }

    private void UpdateRecoveryTimer()
    {
        if (!isUsingRecovery || currentRecovery == null) return;

        recoveryTimer += Time.deltaTime;

        if (recoveryTimer < currentRecovery.HoldTime) return;

        RecoveryItem item = currentRecovery;
        CancelRecoveryUse();
        item.Apply(playerController);
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
            currentEquipped.OnUnequip(playerController);
        }

        ClearEquipment();

        inventory.LoadSaveData(data, equippableDatabase, itemDatabase);

        foreach (InventoryItem inventoryItem in inventory.ItemList)
        {
            EquippableItem item = inventoryItem.Item;

            if (item == null) continue;

            item.Attach(equipPosition);
            item.gameObject.SetActive(false);
        }

        if (data.equippedIndex >= 0 && data.equippedIndex < inventory.ItemList.Count)
        {
            EquipItem(data.equippedIndex);
            return;
        }

        UpdateUpperBodyWeight();
        OnEquipped?.Invoke(currentEquipped);
    }
}
