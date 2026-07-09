using System.Collections.Generic;
using UnityEngine;

public class ObjectDataBase : MonoBehaviour
{
    [SerializeField] private List<WorldObject> prefabList;

    public WorldObject GetPrefab(int itemId)
    {
        foreach (WorldObject prefab in prefabList)
        {
            if (prefab != null && prefab.ItemId == itemId)
                return prefab;
        }
        return null;
    }
}
