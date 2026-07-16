using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> basicItemList = new();

    private List<EquippableItem> itemList = new();
    public IReadOnlyList<EquippableItem> ItemList => itemList;

    private Dictionary<AmmoType, int> ammoDictionary = new();

    public delegate void ChangeSlotHandler();
    public event ChangeSlotHandler OnChanged;

    private void Awake()
    {
        foreach (EquippableItem item in basicItemList)
        {
            if (item == null || itemList.Contains(item)) continue;

            itemList.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    public InventorySaveData CreateSaveData(EquippableItem currentEquipped)
    {
        InventorySaveData data = new()
        {
            itemIdList = new List<int>(),
            equippedIndex = itemList.IndexOf(currentEquipped),
            sackData = null
        };

        foreach (EquippableItem item in itemList)
        {
            if (item == null) continue;
            data.itemIdList.Add(item.ItemId);

            if (item is Sack sack)
            {
                data.sackData = sack.SaveData();
            }
        }

        data.rangedWeaponDataList = new List<RangedWeaponSaveData>();
        foreach (EquippableItem item in itemList)
        {
            RangedWeapon rangedWeapon = item as RangedWeapon;
            if (rangedWeapon == null) continue;

            data.rangedWeaponDataList.Add(new RangedWeaponSaveData
            {
                itemId = rangedWeapon.ItemId,
                currentAmmo = rangedWeapon.CurrentAmmo
            });
        }

        data.ammoDataList = new List<AmmoSaveData>();
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

        foreach (EquippableItem item in itemList)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        itemList.Clear();

        if (data.itemIdList == null)
        {
            OnChanged?.Invoke();
            return;
        }

        foreach (int itemId in data.itemIdList)
        {
            EquippableItem prefab = equippableDatabase.GetPrefab(itemId);
            if (prefab == null) continue;

            EquippableItem item = Instantiate(prefab);

            if (item is Sack sack)
            {
                sack.LoadData(data.sackData, itemDatabase);
            }

            if (item is RangedWeapon rangedWeapon)
            {
                RangedWeaponData weaponDataTable = DataManager.Instance.RangedTable.Get(rangedWeapon.ItemId);
                rangedWeapon.SetUp(weaponDataTable);

                RangedWeaponSaveData weaponData = FindRangedWeaponSaveData(data, rangedWeapon.ItemId);
                if (weaponData != null)
                {
                    rangedWeapon.SetCurrentAmmo(weaponData.currentAmmo);
                }
            }

            itemList.Add(item);
        }

        ammoDictionary.Clear();
        if (data.ammoDataList != null)
        {
            foreach (AmmoSaveData ammoData in data.ammoDataList)
            {
                ammoDictionary[ammoData.ammoType] = ammoData.count;
            }
        }

        OnChanged?.Invoke();
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

    public bool AddItem(EquippableItem newItem, out EquippableItem prevItem)
    {
        prevItem = null;
        if (newItem == null) return false;

        if (newItem is PlaceableItem)
        {
            itemList.Add(newItem);
            OnChanged?.Invoke();
            return true;
        }

        if (FindSameItem(newItem.ItemId) != null) return false;

        if (newItem is IUpgradeable next)
        {
            EquippableItem upgradeItem = FindUpgradeItem(next);
            if (upgradeItem != null)
            {
                if (!CanUpgrade(upgradeItem, newItem)) return false;

                int idx = itemList.IndexOf(upgradeItem);

                prevItem = upgradeItem;
                itemList[idx] = newItem;

                OnChanged?.Invoke();
                return true;
            }
        }
        itemList.Add(newItem);
        OnChanged?.Invoke();

        return true;
    }

    public bool RemoveItem(EquippableItem item)
    {
        if (item == null) return false;
        bool isRemoved = itemList.Remove(item);
        if (isRemoved) OnChanged?.Invoke();

        return isRemoved;
    }

    public void MoveItem(int from, int to)
    {
        if (from == to) return;
        if (from < 0 || to < 0 || from >= itemList.Count || to >= itemList.Count) return;

        EquippableItem item = itemList[from];
        itemList.RemoveAt(from);
        itemList.Insert(to, item);

        OnChanged?.Invoke();
    }

    public EquippableItem SelectItem(int idx)
    {
        if (idx < 0 || itemList.Count <= idx) return null;
        return itemList[idx];
    }

    private bool CanUpgrade(EquippableItem prevItem, EquippableItem newItem)
    {
        if (prevItem is not IUpgradeable previous) return false;
        if (newItem is not IUpgradeable next) return false;
        if (previous.GroupId != next.GroupId) return false;

        return previous.Level < next.Level;
    }

    private EquippableItem FindSameItem(int itemId)
    {
        foreach (EquippableItem item in itemList)
        {
            if (item == null || item.ItemId != itemId) continue;
            return item;
        }
        return null;
    }

    private EquippableItem FindUpgradeItem(IUpgradeable next)
    {
        if (next == null || next.GroupId <= 0) return null;

        foreach (EquippableItem item in itemList)
        {
            if (item is not IUpgradeable current) continue;
            if (current.GroupId != next.GroupId) continue;

            return item;
        }
        return null;
    }

    public void AddAmmo(AmmoType ammoType, int count)
    {
        if (count <= 0) return;

        if (!ammoDictionary.ContainsKey(ammoType)) ammoDictionary[ammoType] = 0;
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
