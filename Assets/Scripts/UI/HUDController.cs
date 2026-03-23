using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EquippedItemUI itemUI;
    [SerializeField] private AmmoCount ammoCount;

    private void Start()
    {
        player.OnEquipped += EquippedItem;
    }

    private void OnDestroy()
    {
        player.OnEquipped -= EquippedItem;
    }

    private void EquippedItem(EquippableItem item)
    {
        itemUI.UpdateUI(item);

        if(item is RangedWeapon rangedWeapon) ammoCount.SetWeapon(rangedWeapon);
        else ammoCount.SetWeapon(null);
    }
}
