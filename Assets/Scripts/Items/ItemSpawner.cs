using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemDataBase itemDataBase;
    [SerializeField] private List<Transform> itemRootList;

    private readonly List<Item> itemList = new();

    private void Awake()
    {
        if (itemRootList == null) return;

        foreach (Transform root in itemRootList)
        {
            if (root == null) continue;

            Item[] items = root.GetComponentsInChildren<Item>();

            foreach (Item item in items)
            {
                Register(item);
            }
        }
    }

    public Item SpawnItem(int itemId, Vector3 position, Quaternion rotation)
    {
        Item prefab = itemDataBase.GetPrefab(itemId);
        if (prefab == null) return null;

        Item item = Instantiate(prefab, position, rotation);
        item.ResetPhysics();
        Register(item);

        return item;
    }

    public void Register(Item item)
    {
        if (item == null || itemList.Contains(item)) return;

        itemList.Add(item);
        item.SetItemSpawner(this);
    }

    public void Unregister(Item item)
    {
        itemList.Remove(item);
    }

    public List<ItemSaveData> CreateSaveData()
    {
        List<ItemSaveData> dataList = new();

        foreach (Item item in itemList)
        {
            if (item == null || !item.gameObject.activeInHierarchy) continue;

            Vector3 position = item.transform.position;
            Vector3 rotation = item.transform.eulerAngles;

            dataList.Add(new ItemSaveData
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

    public void LoadSaveData(List<ItemSaveData> dataList)
    {
        Clear();
        if (dataList == null) return;

        foreach (ItemSaveData data in dataList)
        {
            Vector3 position = new(data.positionX, data.positionY, data.positionZ);
            Quaternion rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);

            SpawnItem(data.itemId, position, rotation);
        }
    }

    private void Clear()
    {
        List<Item> tempList = new(itemList);

        foreach (Item item in tempList)
        {
            if (item == null) continue;

            Destroy(item.gameObject);
        }

        itemList.Clear();
    }
}
