using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> basicItemList = new();

    private readonly List<InventoryItem> itemList = new();
    public IReadOnlyList<InventoryItem> ItemList => itemList;

    private readonly Dictionary<AmmoType, int> ammoDictionary = new();

    public event Action OnChanged;

    private void Awake()
    {
        foreach (InventoryItem basicItem in basicItemList)
        {
            if (basicItem == null || basicItem.Item == null) continue;

            InventoryItem sameItem = FindItem(basicItem.Item.ItemId);
            if (sameItem != null)
            {
                sameItem.TryAdd(basicItem.Count);
                continue;
            }

            itemList.Add(new InventoryItem(basicItem.Item, basicItem.Count));
            basicItem.Item.gameObject.SetActive(false);
        }
    }

    public bool AddItem(EquippableItem newItem, out EquippableItem prevItem, out bool isStacked)
    {
        prevItem = null;
        isStacked = false;

        if (newItem == null) return false;

        // 중첩
        InventoryItem sameItem = FindItem(newItem.ItemId);
        if (sameItem != null)
        {
            if (!sameItem.TryAdd(1)) return false;

            isStacked = true;
            OnChanged?.Invoke();
            return true;
        }

        // 업그레이드
        if (newItem is IUpgradeable next)
        {
            InventoryItem upgradeItem = FindUpgradeItem(next);
            if (upgradeItem != null)
            {
                if (!CanUpgrade(upgradeItem.Item, newItem)) return false;

                int index = itemList.IndexOf(upgradeItem);

                prevItem = upgradeItem.Item;
                itemList[index] = new InventoryItem(newItem);

                OnChanged?.Invoke();
                return true;
            }
        }

        itemList.Add(new InventoryItem(newItem));
        OnChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(EquippableItem item, out bool isEmpty)
    {
        isEmpty = false;

        if (item == null) return false;

        InventoryItem inventoryItem = FindItem(item.ItemId);
        if (inventoryItem == null || !inventoryItem.TryRemove(1)) return false;

        isEmpty = inventoryItem.Count <= 0;
        if (isEmpty)
        {
            itemList.Remove(inventoryItem);
        }

        OnChanged?.Invoke();
        return true;
    }

    public int GetItemCount(EquippableItem item)
    {
        if (item == null) return 0;

        InventoryItem inventoryItem = FindItem(item.ItemId);
        if (inventoryItem == null) return 0;

        return inventoryItem.Count;
    }

    public void MoveItem(int from, int to)
    {
        if (from == to) return;
        if (!IsValidIndex(from) || !IsValidIndex(to)) return;

        InventoryItem item = itemList[from];
        itemList.RemoveAt(from);
        itemList.Insert(to, item);

        OnChanged?.Invoke();
    }

    public EquippableItem SelectItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        return itemList[index].Item;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < itemList.Count;
    }

    private InventoryItem FindItem(int itemId)
    {
        foreach (InventoryItem inventoryItem in itemList)
        {
            if (inventoryItem.Item == null) continue;
            if (inventoryItem.Item.ItemId == itemId) return inventoryItem;
        }

        return null;
    }

    private InventoryItem FindUpgradeItem(IUpgradeable next)
    {
        if (next == null || next.GroupId <= 0) return null;

        foreach (InventoryItem inventoryItem in itemList)
        {
            if (inventoryItem.Item is not IUpgradeable current) continue;
            if (current.GroupId == next.GroupId) return inventoryItem;
        }

        return null;
    }

    private bool CanUpgrade(EquippableItem currentItem, EquippableItem newItem)
    {
        if (currentItem is not IUpgradeable current) return false;
        if (newItem is not IUpgradeable next) return false;

        return current.GroupId == next.GroupId && current.Level < next.Level;
    }

    public InventorySaveData CreateSaveData(EquippableItem currentEquipped)
    {
        InventorySaveData data = new()
        {
            itemIdList = new List<int>(),
            equippedIndex = itemList.FindIndex(item => item.Item == currentEquipped),
            sackData = null,
            rangedWeaponDataList = new List<RangedWeaponSaveData>(),
            ammoDataList = new List<AmmoSaveData>()
        };

        foreach (InventoryItem inventoryItem in itemList)
        {
            EquippableItem item = inventoryItem.Item;
            if (item == null) continue;

            for (int i = 0; i < inventoryItem.Count; i++)
            {
                data.itemIdList.Add(item.ItemId);
            }

            if (item is Sack sack)
            {
                data.sackData = sack.SaveData();
            }

            if (item is RangedWeapon rangedWeapon)
            {
                data.rangedWeaponDataList.Add(new RangedWeaponSaveData
                {
                    itemId = rangedWeapon.ItemId,
                    currentAmmo = rangedWeapon.CurrentAmmo
                });
            }
        }

        foreach (var ammo in ammoDictionary)
        {
            data.ammoDataList.Add(new AmmoSaveData
            {
                ammoType = ammo.Key,
                count = ammo.Value
            });
        }

        return data;
    }

    public void LoadSaveData(InventorySaveData data, EquippableDatabase equippableDatabase, ItemDataBase itemDatabase)
    {
        if (data == null || equippableDatabase == null || itemDatabase == null) return;

        ClearItems();

        foreach (int itemId in data.itemIdList)
        {
            InventoryItem sameItem = FindItem(itemId);
            if (sameItem != null)
            {
                sameItem.TryAdd(1);
                continue;
            }

            EquippableItem prefab = equippableDatabase.GetPrefab(itemId);
            EquippableItem item = Instantiate(prefab);

            if (item is Sack sack) sack.LoadData(data.sackData, itemDatabase);

            if (item is RangedWeapon rangedWeapon)
            {
                RangedWeaponData weaponData = DataManager.Instance.RangedTable.Get(rangedWeapon.ItemId);
                rangedWeapon.SetUp(weaponData);

                RangedWeaponSaveData saveData = FindRangedWeaponSaveData(data, rangedWeapon.ItemId);
                if (saveData != null)
                {
                    rangedWeapon.SetCurrentAmmo(saveData.currentAmmo);
                }
            }

            if (item != null)
            {
                itemList.Add(new InventoryItem(item));
            }
        }


        LoadAmmoData(data.ammoDataList);
        OnChanged?.Invoke();
    }

    private void ClearItems()
    {
        foreach (InventoryItem inventoryItem in itemList)
        {
            if (inventoryItem.Item != null)
            {
                Destroy(inventoryItem.Item.gameObject);
            }
        }
        itemList.Clear();
    }

    private void LoadAmmoData(List<AmmoSaveData> ammoDataList)
    {
        ammoDictionary.Clear();
        if (ammoDataList == null) return;

        foreach (AmmoSaveData ammoData in ammoDataList)
        {
            ammoDictionary[ammoData.ammoType] = ammoData.count;
        }
    }

    private RangedWeaponSaveData FindRangedWeaponSaveData(InventorySaveData data, int itemId)
    {
        if (data.rangedWeaponDataList == null) return null;

        foreach (RangedWeaponSaveData weaponData in data.rangedWeaponDataList)
        {
            if (weaponData != null && weaponData.itemId == itemId)
            {
                return weaponData;
            }
        }

        return null;
    }

    public void AddAmmo(AmmoType ammoType, int count)
    {
        if (count <= 0) return;

        if (!ammoDictionary.ContainsKey(ammoType))
        {
            ammoDictionary[ammoType] = 0;
        }

        ammoDictionary[ammoType] += count;
    }

    public int UseAmmo(AmmoType ammoType, int count)
    {
        if (count <= 0) return 0;

        int current = GetAmmoCount(ammoType);
        int used = Mathf.Min(current, count);

        ammoDictionary[ammoType] = current - used;
        return used;
    }

    public int GetAmmoCount(AmmoType ammoType)
    {
        return ammoDictionary.TryGetValue(ammoType, out int count) ? count : 0;
    }
}
