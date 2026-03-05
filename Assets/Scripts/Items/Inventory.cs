using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> basicItems = new();
    private List<EquippableItem> items = new();

    private void Awake()
    {
        foreach(EquippableItem item in basicItems)
        {
            items.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    public bool AddItem(EquippableItem item)
    {
        if(item == null || items.Exists(x => x.ItemName == item.ItemName)) 
        {
            // Debug.Log(item + "획득 실패!");
            return false;
        }

        items.Add(item);
        // Debug.Log(item + "획득!");
        return true;
    }

    public bool RemoveItem(EquippableItem item)
    {
        if(item == null) return false;
        return items.Remove(item);
    }

    public EquippableItem SelectItem(int idx)
    {
        if(idx < 0 || idx >= items.Count) return null;
        return items[idx];
    }
}
