using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemGuideUI itemGuideUI;
    [SerializeField] private AmmoCount ammoCount;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private ItemHoverUI itemHoverUI;

    private EquippableItem currentEquipped;
    private EquippableItem currentHoverEquippable;
    private Item currentItem;

    private void Awake()
    {
        player.OnEquipped += EquippedItem;
        player.OnSackChanged += RefreshItemGuideUI;

        inventoryUI.OnSelectSlot += SelectSlot;

        gameInput.OnHoverItem += ShowItem;
        gameInput.OnHoverEquippable += ShowEquippable;
        gameInput.OnExitItem += HideItem;
    }

    private void OnDestroy()
    {
        player.OnEquipped -= EquippedItem;
        player.OnSackChanged -= RefreshItemGuideUI;

        inventoryUI.OnSelectSlot -= SelectSlot;

        gameInput.OnHoverItem -= ShowItem;
        gameInput.OnHoverEquippable -= ShowEquippable;
        gameInput.OnExitItem -= HideItem;
    }

    private void EquippedItem(EquippableItem item)
    {
        currentEquipped = item;
        RefreshItemGuideUI();

        if (item is RangedWeapon rangedWeapon) ammoCount.SetWeapon(rangedWeapon);
        else ammoCount.SetWeapon(null);
    }

    private void SelectSlot(int idx)
    {
        player.EquipItem(idx);
    }

    private void ShowItem(Item item)
    {
        currentItem = item;
        currentHoverEquippable = null;

        itemHoverUI.ShowUI(item);
        RefreshItemGuideUI();
    }

    private void ShowEquippable(EquippableItem item)
    {
        currentItem = null;
        currentHoverEquippable = item;

        itemHoverUI.ShowUI(item);
        RefreshItemGuideUI();
    }

    private void HideItem()
    {
        currentItem = null;
        currentHoverEquippable = null;

        itemHoverUI.HideUI();
        RefreshItemGuideUI();
    }

    private void RefreshItemGuideUI()
    {
        itemGuideUI.UpdateUI(currentEquipped, currentItem, currentHoverEquippable);
    }
}
