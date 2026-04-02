using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem, ISubject
{
    [SerializeField] private SackData sackData;

    private Stack<Item> itemStack = new();
    private readonly List<IObserver> ObserverList = new();

    public SackLevel Level => sackData.level;
    public override bool CanDrop => false;

    public int Capacity => sackData.capacity;
    public int Count => itemStack.Count;
    public bool IsFull => itemStack.Count >= Capacity;
    public bool IsEmpty => itemStack.Count == 0;

    public override void OnEquip(PlayerController player)
    {
        player.SetSack(this);
        gameObject.SetActive(true);
        NotifyObservers();
    }

    public override void OnUnequip(PlayerController player)
    {
        player.SetSack(null);
        gameObject.SetActive(false);
    }

    public bool AddItem(Item item)
    {
        if (item == null || IsFull) return false;

        itemStack.Push(item);
        item.transform.SetParent(transform);
        item.gameObject.SetActive(false);

        NotifyObservers();
        return true;
    }

    public Item DropItem()
    {
        if (IsEmpty) return null;

        Item item = itemStack.Pop();
        item.transform.position = transform.position + Vector3.up * 0.7f;
        item.transform.SetParent(null);
        item.gameObject.SetActive(true);

        NotifyObservers();
        return item;
    }

    public void MoveItems(Sack nextSack)
    {
        if (nextSack == null) return;

        Stack<Item> temp = new();

        while (itemStack.Count > 0)
        {
            temp.Push(itemStack.Pop());
        }

        while (temp.Count > 0)
        {
            Item item = temp.Pop();
            nextSack.AddItem(item);
        }
    }

    public void AddObserver(IObserver observer)
    {
        ObserverList.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        ObserverList.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in ObserverList)
        {
            observer.Notify();
        }
    }
}
