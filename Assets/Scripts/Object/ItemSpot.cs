using System.Collections.Generic;
using UnityEngine;

public class ItemSpot : MonoBehaviour
{
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private List<int> itemIdList;
    [SerializeField] private int level1Item;

    public void SpawnItem(int level, System.Random random, ItemRegistry itemRegistry)
    {
        int itemId;

        if (level == 1)
        {
            itemId = level1Item;
        }
        else
        {
            if (itemIdList == null || itemIdList.Count == 0)
            {
                Debug.LogWarning($"{name}: Item ID List is empty.", this);
                return;
            }

            itemId = itemIdList[random.Next(0, itemIdList.Count)];
        }

        Item item = itemRegistry.SpawnItem(itemId, itemSpawnPoint.position, itemSpawnPoint.rotation);

        if (item == null)
        {
            Debug.LogWarning($"{name}: Failed to spawn Item {itemId}.", this);
        }
    }
}
