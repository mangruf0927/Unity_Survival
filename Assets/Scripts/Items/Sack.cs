using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem, ISubject
{
    [SerializeField] private int sackId;

    private string sackName;
    private SackLevel sackLevel;
    private int capacity;

    private Stack<Item> itemStack = new();
    private readonly List<IObserver> ObserverList = new();

    public SackLevel Level => sackLevel;
    public int Capacity => capacity;
    public override bool CanDrop => canDrop;

    public int Count => itemStack.Count;
    public bool IsFull => itemStack.Count >= Capacity;
    public bool IsEmpty => itemStack.Count == 0;

    private IEnumerator Start()
    {
        if (!DataManager.Instance.IsLoaded)
        {
            yield return DataManager.Instance.LoadAll();
        }

        SackDataTable data = DataManager.Instance.SackTable.Get(sackId);
        SetUp(data);
    }

    public void SetUp(SackDataTable data)
    {
        sackName = data.Name;
        sackLevel = data.Level;
        capacity = data.Capacity;
        canDrop = data.CanDrop;
    }

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
