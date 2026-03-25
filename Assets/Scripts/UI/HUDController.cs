using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    
    [SerializeField] private EquippedItemUI itemUI;
    [SerializeField] private AmmoCount ammoCount;
    [SerializeField] private InventoryUI inventoryUI;

    private void Start()
    {
        player.OnEquipped += EquippedItem;
        inventoryUI.OnSelectSlot += SelectSlot;
    }

    private void OnDestroy()
    {
        player.OnEquipped -= EquippedItem;
        inventoryUI.OnSelectSlot -= SelectSlot;
    }

    private void EquippedItem(EquippableItem item)
    {
        itemUI.UpdateUI(item);

        if(item is RangedWeapon rangedWeapon) ammoCount.SetWeapon(rangedWeapon);
        else ammoCount.SetWeapon(null);
    }

    private void SelectSlot(int idx)
    {
        player.EquipItem(idx);
    }
}
