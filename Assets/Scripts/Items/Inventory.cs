using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<EquippableItem> items = new();

    public void AddItem(EquippableItem item)
    {
        if(item == null) return;
        items.Add(item);
    }

    public void RemoveItem(EquippableItem item)
    {
        if(item == null) return;
        items.Remove(item);
    }

    public EquippableItem SelectItem(int idx)
    {
        if(idx < 0 || idx >= items.Count) return null;
        return items[idx];
    }
}
