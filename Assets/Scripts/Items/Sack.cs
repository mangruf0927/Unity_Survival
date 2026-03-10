using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem
{   
    [SerializeField] private SackData sackData;

    private Stack<Item> items = new();
    
    public SackLevel Level => sackData.level;
    public int Capacity => sackData.capacity;
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

    public void MoveItems(Sack nextSack)
    {
        if(nextSack == null) return;

        Stack<Item> temp = new();

        while(items.Count > 0)
        {
            temp.Push(items.Pop());
        }

        while(temp.Count > 0)
        {
            Item item = temp.Pop();
            nextSack.AddItem(item);
        }
    }
}
