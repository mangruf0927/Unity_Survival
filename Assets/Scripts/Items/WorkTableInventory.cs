using System.Collections.Generic;
using UnityEngine;

public class WorkTableInventory : MonoBehaviour, ISubject, IInteractable
{
    [SerializeField] private WorkTableItem[] itemList;
    [SerializeField] private WorkTableInventoryUI inventoryUI;

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Transform uiPoint;

    private int wood = 0;
    private int iron = 0;

    public int Wood => wood;
    public int Iron => iron;

    public WorkTableItem[] ItemList => itemList;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

    private readonly List<IObserver> ObserverList = new();

    public void AddMaterial(MaterialType type, int amount)
    {
        if (type == MaterialType.WOOD)
        {
            wood += amount;
        }
        else if (type == MaterialType.IRON)
        {
            iron += amount;
        }
        else return;

        NotifyObservers();
    }

    public bool CanBuy(WorkTableItem item)
    {
        return wood >= item.needWood && iron >= item.needIron;
    }

    public bool BuyItem(WorkTableItem item)
    {
        if (!CanBuy(item)) return false;

        wood -= item.needWood;
        iron -= item.needIron;

        NotifyObservers();
        return true;
    }

    public bool CanInteract(PlayerController player)
    {
        return inventoryUI != null && !inventoryUI.IsOpen;
    }

    public void Interact(PlayerController player)
    {
        inventoryUI.OpenUI();
    }

    public void AddObserver(IObserver observer)
    {
        ObserverList.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        if (observer == null) return;
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
