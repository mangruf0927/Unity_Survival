using System.Collections.Generic;
using UnityEngine;

public class WorkTableInventory : MonoBehaviour, ISubject, IInteractable
{
    [SerializeField] private WorkTableItem[] itemList;
    [SerializeField] private WorkTableInventoryUI inventoryUI;

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Transform uiPoint;

    private PlayerController currentPlayer;

    private int iron = 0;
    private int wood = 0;
    private int currentLevel = 1;

    public int Iron => iron;
    public int Wood => wood;
    public int CurrentLevel => currentLevel;

    public WorkTableItem[] ItemList => itemList;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

    private readonly Dictionary<WorkTableItem, int> purchaseDictionary = new();
    private readonly List<IObserver> ObserverList = new();

    public WorkTableSaveData CreateSaveData()
    {
        WorkTableSaveData data = new()
        {
            iron = iron,
            wood = wood,
            currentLevel = currentLevel,
            purchaseDataList = new List<PurchaseSaveData>()
        };

        for (int i = 0; i < itemList.Length; i++)
        {
            WorkTableItem item = itemList[i];
            int count = GetPurchaseCount(item);

            if (count <= 0) continue;

            data.purchaseDataList.Add(new PurchaseSaveData
            {
                itemIndex = i,
                purchaseCount = count
            });
        }

        return data;
    }

    public void LoadSaveData(WorkTableSaveData data)
    {
        if (data == null) return;

        iron = Mathf.Max(0, data.iron);
        wood = Mathf.Max(0, data.wood);
        currentLevel = Mathf.Max(1, data.currentLevel);

        purchaseDictionary.Clear();

        if (data.purchaseDataList != null)
        {
            foreach (PurchaseSaveData purchaseData in data.purchaseDataList)
            {
                if (purchaseData.itemIndex < 0 || purchaseData.itemIndex >= itemList.Length) continue;

                WorkTableItem item = itemList[purchaseData.itemIndex];
                purchaseDictionary[item] = Mathf.Max(0, purchaseData.purchaseCount);
            }
        }

        NotifyObservers();
    }

    public void AddMaterial(MaterialType type, int amount)
    {
        if (type == MaterialType.IRON)
        {
            iron += amount;
        }
        else if (type == MaterialType.WOOD)
        {
            wood += amount;
        }
        else return;

        NotifyObservers();
    }

    public bool IsUnlocked(WorkTableItem item)
    {
        return item.requiredLevel <= currentLevel;
    }

    public bool IsSoldOut(WorkTableItem item)
    {
        if (item.purchaseLimit < 0) return false;
        return GetPurchaseCount(item) >= item.purchaseLimit;
    }

    public bool HasMaterial(WorkTableItem item)
    {
        return iron >= item.needIron && wood >= item.needWood;
    }

    public bool CanBuy(WorkTableItem item)
    {
        return IsUnlocked(item)
               && !IsSoldOut(item)
               && HasMaterial(item);
    }

    public bool BuyItem(WorkTableItem item)
    {
        if (!CanBuy(item)) return false;

        if (!item.unlocksNextLevel)
        {
            if (item.itemPrefab == null || currentPlayer == null) return false;

            PlaceableItem newItem = Instantiate(item.itemPrefab);

            if (!currentPlayer.GetEquippableItem(newItem))
            {
                Destroy(newItem.gameObject);
                return false;
            }
        }

        iron -= item.needIron;
        wood -= item.needWood;

        purchaseDictionary[item] = GetPurchaseCount(item) + 1;

        if (item.unlocksNextLevel)
        {
            currentLevel = Mathf.Max(currentLevel, item.requiredLevel + 1);
        }

        NotifyObservers();
        return true;
    }

    public int GetPurchaseCount(WorkTableItem item)
    {
        return purchaseDictionary.TryGetValue(item, out int count) ? count : 0;
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
