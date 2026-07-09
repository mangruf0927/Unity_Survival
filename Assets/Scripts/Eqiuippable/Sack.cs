using System.Collections.Generic;
using UnityEngine;

public class Sack : EquippableItem, ISubject
{
    private SackLevel sackLevel;
    private int capacity;

    private readonly Stack<Item> itemStack = new();
    private readonly List<IObserver> ObserverList = new();

    public SackLevel Level => sackLevel;
    public int Capacity => capacity;
    public override bool CanDrop => canDrop;

    public int Count => itemStack.Count;
    public bool IsFull => itemStack.Count >= Capacity;
    public bool IsEmpty => itemStack.Count == 0;

    private void Awake()
    {
        SackData data = DataManager.Instance.SackTable.Get(ItemId);
        SetUp(data);
    }

    public void SetUp(SackData data)
    {
        itemName = data.Name;
        sackLevel = data.Level;
        capacity = data.Capacity;
        canDrop = data.CanDrop;
    }

    public SackSaveData SaveData()
    {
        SackSaveData data = new()
        {
            itemIdList = new List<int>()
        };

        foreach (Item item in itemStack)
        {
            if (item == null) continue;
            data.itemIdList.Add(item.ItemId);
        }
        return data;
    }

    public void LoadData(SackSaveData data, ItemDataBase database)
    {
        if (data == null || data.itemIdList == null || database == null) return;

        SackData sackData = DataManager.Instance.SackTable.Get(ItemId);
        SetUp(sackData);

        for (int i = data.itemIdList.Count - 1; i >= 0; i--)
        {
            Item prefab = database.GetPrefab(data.itemIdList[i]);
            if (prefab == null) continue;

            Item item = Instantiate(prefab);
            AddItem(item);
        }

        NotifyObservers();
    }

    public override void OnEquip(PlayerController player)
    {
        player.SetSack(this);
        gameObject.SetActive(true);

        SackItemCount itemCount = GetComponentInChildren<SackItemCount>(true);
        if (itemCount != null) itemCount.gameObject.SetActive(true);

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

        item.UnregisterItem();
        item.ResetPhysics();

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
        item.transform.SetParent(null);
        item.transform.position = transform.position + Vector3.up * 0.7f;
        item.ResetPhysics();
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
