using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private EquippableItem item;
    [SerializeField] private int count = 1;

    public EquippableItem Item => item;
    public int Count => count;

    public InventoryItem(EquippableItem item, int count = 1)
    {
        this.item = item;
        this.count = Mathf.Clamp(count, 1, item.MaxCount);
    }

    public bool TryAdd(int amount)
    {
        if (amount <= 0) return false;
        if (Count + amount > Item.MaxCount) return false;

        count += amount;
        return true;
    }

    public bool TryRemove(int amount)
    {
        if (amount <= 0) return false;
        if (Count < amount) return false;

        count -= amount;
        return true;
    }
}
