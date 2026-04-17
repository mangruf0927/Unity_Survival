using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EquippedItemUI itemUI;
    [SerializeField] private AmmoCount ammoCount;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private ItemHoverUI itemHoverUI;

    private void Start()
    {
        player.OnEquipped += EquippedItem;
        inventoryUI.OnSelectSlot += SelectSlot;

        gameInput.OnHoverItem += ShowItem;
        gameInput.OnExitItem += HideItem;
    }

    private void OnDestroy()
    {
        player.OnEquipped -= EquippedItem;
        inventoryUI.OnSelectSlot -= SelectSlot;

        gameInput.OnHoverItem -= ShowItem;
        gameInput.OnExitItem -= HideItem;
    }

    private void EquippedItem(EquippableItem item)
    {
        itemUI.UpdateUI(item);

        if (item is RangedWeapon rangedWeapon) ammoCount.SetWeapon(rangedWeapon);
        else ammoCount.SetWeapon(null);
    }

    private void SelectSlot(int idx)
    {
        player.EquipItem(idx);
    }

    private void ShowItem(Item item)
    {
        itemHoverUI.ShowUI(item);
    }

    private void HideItem()
    {
        itemHoverUI.HideUI();
    }
}
