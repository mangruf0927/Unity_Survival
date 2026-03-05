using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem
{
    public enum SackLevel { OLD, GOOD, GIANT }

    [SerializeField] private SackLevel level;
    
    private Stack<ItemTest> items = new();
    
    public int Capacity => GetCapacity();
    public int Count => items.Count;
    public bool IsFull => items.Count >= Capacity;
    public bool IsEmpty => items.Count == 0;
    public override bool CanDrop => false;

    public override void OnEquip(PlayerController player)
    {
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        gameObject.SetActive(false);
    }

    private int GetCapacity()
    {
        if(level == SackLevel.OLD) return 5;
        else if(level == SackLevel.GOOD) return 15;
        else return 25;
    }

    public bool AddItem(ItemTest item)
    {
        if(item == null || IsFull) return false;

        items.Push(item);
        return true;
    }

    public ItemTest RemoveItem()
    {
        if(IsEmpty) return null;

        return items.Pop();
    }
}
