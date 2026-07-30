using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] private List<Transform> itemSpawnPointList;
    [SerializeField] private List<Transform> chestSpawnPointList;

    [SerializeField] private List<int> itemIdList;

    [SerializeField] private int minItemSpawnCount = 2;
    [SerializeField] private int maxItemSpawnCount = 4;

    public void SpawnItems(System.Random random, ItemRegistry itemRegistry)
    {
        if (itemIdList == null || itemIdList.Count == 0)
        {
            Debug.LogWarning($"{name}: Item ID List is empty", this);
            return;
        }

        if (itemSpawnPointList == null || itemSpawnPointList.Count == 0)
        {
            Debug.LogWarning($"{name}: Item Spawn List is empty.", this);
            return;
        }

        List<Transform> availablePointList = new(itemSpawnPointList);

        int minCnt = Mathf.Max(0, minItemSpawnCount);
        int maxCnt = Mathf.Max(minCnt, maxItemSpawnCount);

        maxCnt = Mathf.Min(maxCnt, availablePointList.Count);
        minCnt = Mathf.Min(minCnt, maxCnt);

        int spawnCount = random.Next(minCnt, maxCnt + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int idx = random.Next(0, availablePointList.Count);
            Transform spawnPoint = availablePointList[idx];

            availablePointList.RemoveAt(idx);

            if (spawnPoint == null)
            {
                i--;
                continue;
            }

            int itemId = itemIdList[random.Next(0, itemIdList.Count)];
            Item item = itemRegistry.SpawnItem(itemId, spawnPoint.position, spawnPoint.rotation);

            if (item == null)
            {
                Debug.LogWarning($"{name}: Failed to spawn {itemId} item", this);
            }
        }
    }

    public void SpawnChests(System.Random random, GameObject chestPrefab, ObjectRegistry objectRegistry)
    {
        int index = random.Next(0, chestSpawnPointList.Count);
        Transform spawnPoint = chestSpawnPointList[index];

        GameObject chestObject = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        if (!chestObject.TryGetComponent(out Chest chest))
        {
            Debug.LogWarning($"{chestObject.name}: Chest component is null.", chestObject);
            Destroy(chestObject);
            return;
        }

        chest.SetInstanceId(objectRegistry.CreateInstanceId());
        objectRegistry.Register(chest);
    }
}
