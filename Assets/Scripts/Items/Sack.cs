using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem
{
    public enum SackLevel { OLD, GOOD, GIANT }

    [SerializeField] private SackLevel level;
    
    private Stack<Item> items = new();
    
    public int Capacity => GetCapacity();
    public int Count => items.Count;
    public bool IsFull => items.Count >= Capacity;
    public bool IsEmpty => items.Count == 0;
    public override bool CanDrop => false;

    public override void OnEquip(PlayerController player)
    {
        gameObject.SetActive(true);
        player.currentSack = this;
    }

    public override void OnUnequip(PlayerController player)
    {
        gameObject.SetActive(false);
        player.currentSack = null;
    }

    private int GetCapacity()
    {
        if(level == SackLevel.OLD) return 5;
        else if(level == SackLevel.GOOD) return 15;
        else return 25;
    }

    public bool AddItem(Item item)
    {
        if(item == null || IsFull) return false;

        items.Push(item);

        item.transform.SetParent(transform);
        item.gameObject.SetActive(false);
        return true;
    }

    public Item DropItem()
    {
        if(IsEmpty) return null;

        Item item = items.Pop();
        item.transform.position = transform.position + Vector3.up * 0.7f;
        item.transform.SetParent(null);
        item.gameObject.SetActive(true);

        return item;
    }
}
