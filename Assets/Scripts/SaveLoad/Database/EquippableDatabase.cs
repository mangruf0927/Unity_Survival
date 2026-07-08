using System.Collections.Generic;
using UnityEngine;

public class EquippableDatabase : MonoBehaviour
{
    [SerializeField] private List<EquippableItem> prefabList;

    public EquippableItem GetPrefab(int itemId)
    {
        foreach (EquippableItem prefab in prefabList)
        {
            if (prefab != null && prefab.ItemId == itemId)
                return prefab;
        }

        return null;
    }
}
