using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> basicItemList = new();
    
    private List<EquippableItem> itemList = new();
    public IReadOnlyList<EquippableItem> ItemList => itemList;

    private void Awake()
    {
        foreach (EquippableItem item in basicItemList)
        {
            itemList.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    public bool AddItem(EquippableItem newItem, out EquippableItem prevItem)
    {
        prevItem = null;
        if (newItem == null) return false;

        for (int i = 0; i < itemList.Count; i++)
        {
            EquippableItem item = itemList[i];

            if (item.ItemType != newItem.ItemType) continue;
            if (!CanUpgrade(item, newItem)) return false;

            prevItem = item;
            itemList[i] = newItem;
            return true;
        }
        
        itemList.Add(newItem);
        return true;
    }

    public bool RemoveItem(EquippableItem item)
    {
        if (item == null) return false;
        return itemList.Remove(item);
    }

    public EquippableItem SelectItem(int idx)
    {
        if (idx < 0 || itemList.Count <= idx) return null;
        return itemList[idx];
    }

    private bool CanUpgrade(EquippableItem prevItem, EquippableItem newItem)
    {
        if (prevItem is MeleeWeapon prevMelee && newItem is MeleeWeapon newMelee)
        {
            return prevMelee.Level < newMelee.Level;
        }

        if (prevItem is Sack prevSack && newItem is Sack newSack)
        {
            return prevSack.Level < newSack.Level;
        }

        return false;
    }
}
