using System.Collections.Generic;
using UnityEngine;

public class EquippableSpawner : MonoBehaviour
{
    [SerializeField] private EquippableDatabase equippableDatabase;
    [SerializeField] private Transform equippableRoot;

    private readonly List<EquippableItem> equippableList = new();

    private void Awake()
    {
        if (equippableRoot == null) return;

        EquippableItem[] items = equippableRoot.GetComponentsInChildren<EquippableItem>();

        foreach (EquippableItem item in items)
        {
            Register(item);
        }
    }

    public EquippableItem SpawnItem(int itemId, Vector3 position, Quaternion rotation)
    {
        EquippableItem prefab = equippableDatabase.GetPrefab(itemId);
        if (prefab == null) return null;

        EquippableItem item = Instantiate(prefab, position, rotation);
        Register(item);

        return item;
    }

    public void Register(EquippableItem item)
    {
        if (item == null || equippableList.Contains(item)) return;

        equippableList.Add(item);
        item.SetEquippableSpawner(this);
    }

    public void Unregister(EquippableItem item)
    {
        if (item == null) return;

        equippableList.Remove(item);
    }

    public List<EquippableSaveData> CreateSaveData()
    {
        List<EquippableSaveData> dataList = new();

        foreach (EquippableItem item in equippableList)
        {
            if (item == null) continue;
            if (!item.gameObject.activeInHierarchy) continue;

            Vector3 position = item.transform.position;
            Vector3 rotation = item.transform.eulerAngles;

            dataList.Add(new EquippableSaveData
            {
                itemId = item.ItemId,
                positionX = position.x,
                positionY = position.y,
                positionZ = position.z,
                rotationX = rotation.x,
                rotationY = rotation.y,
                rotationZ = rotation.z
            });
        }

        return dataList;
    }

    public void LoadSaveData(List<EquippableSaveData> dataList)
    {
        Clear();

        if (dataList == null) return;

        foreach (EquippableSaveData data in dataList)
        {
            Vector3 position = new(data.positionX, data.positionY, data.positionZ);
            Quaternion rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);

            SpawnItem(data.itemId, position, rotation);
        }
    }

    private void Clear()
    {
        List<EquippableItem> copyList = new(equippableList);

        foreach (EquippableItem item in copyList)
        {
            if (item == null) continue;

            Destroy(item.gameObject);
        }

        equippableList.Clear();
    }
}