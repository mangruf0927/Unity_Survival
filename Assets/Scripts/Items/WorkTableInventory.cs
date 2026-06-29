using System.Collections.Generic;
using UnityEngine;

public class WorkTableInventory : MonoBehaviour, ISubject, IInteractable
{
    [SerializeField] private WorkTableItem[] itemList;
    [SerializeField] private WorkTableInventoryUI inventoryUI;

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Transform uiPoint;

    private PlayerController currentPlayer;

    private int wood = 0;
    private int iron = 0;
    private int currentLevel = 1;

    public int Wood => wood;
    public int Iron => iron;
    public int CurrentLevel => currentLevel;

    public WorkTableItem[] ItemList => itemList;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

    private readonly Dictionary<WorkTableItem, int> purchaseDictionary = new();
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

    public bool IsUnlocked(WorkTableItem item)
    {
        return item.requiredLevel <= currentLevel;
    }

    public bool CanBuy(WorkTableItem item)
    {
        return IsUnlocked(item) && !IsSoldOut(item) &&
               wood >= item.needWood && iron >= item.needIron;
    }

    public bool BuyItem(WorkTableItem item)
    {
        if (!CanBuy(item)) return false;
        if (item.itemPrefab == null || currentPlayer == null) return false;

        PlaceableItem newItem = Instantiate(item.itemPrefab);

        if (!currentPlayer.GetEquippableItem(newItem))
        {
            Destroy(newItem.gameObject);
            return false;
        }

        wood -= item.needWood;
        iron -= item.needIron;

        purchaseDictionary[item] = GetPurchaseCount(item) + 1;

        if (item.unlocksNextLevel) currentLevel++;

        NotifyObservers();
        return true;
    }

    public int GetPurchaseCount(WorkTableItem item)
    {
        return purchaseDictionary.TryGetValue(item, out int count) ? count : 0;
    }

    public bool IsSoldOut(WorkTableItem item)
    {
        if (item.purchaseLimit < 0) return false;
        return GetPurchaseCount(item) >= item.purchaseLimit;
    }

    public bool CanInteract(PlayerController player)
    {
        return player != null && inventoryUI != null && !inventoryUI.IsOpen;
    }

    public void Interact(PlayerController player)
    {
        if (!CanInteract(player)) return;

        currentPlayer = player;
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
