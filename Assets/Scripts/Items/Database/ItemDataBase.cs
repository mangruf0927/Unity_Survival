using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    [SerializeField] private List<Item> prefabList;

    public Item GetPrefab(int itemId)
    {
        foreach (Item prefab in prefabList)
        {
            if (prefab != null && prefab.ItemId == itemId)
                return prefab;
        }
        return null;
    }
}
