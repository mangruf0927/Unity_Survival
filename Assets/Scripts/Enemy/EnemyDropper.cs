using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItemInfo
{
    public int itemId;
    public int minCount;
    public int maxCount;
    [Range(0f, 1f)] public float chance;
}

public class EnemyDropper : MonoBehaviour
{
    [SerializeField] private List<DropItemInfo> dropItemList;

    private ItemRegistry itemRegistry;
    private ItemDataBase itemDataBase;

    public void SetUp(ItemDataBase database, ItemRegistry registry)
    {
        itemDataBase = database;
        itemRegistry = registry;
    }

    public void DropItems()
    {
        if (itemDataBase == null || dropItemList == null) return;

        foreach (DropItemInfo dropItem in dropItemList)
        {
            if (dropItem == null || Random.value > dropItem.chance) continue;
            int count = Random.Range(dropItem.minCount, dropItem.maxCount + 1);

            for (int i = 0; i < count; i++)
                SpawnItem(dropItem.itemId);
        }
    }

    private void SpawnItem(int itemId)
    {
        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(0.5f, 1.5f);
        Vector3 dropPosition = transform.position + new Vector3(offset.x, 1f, offset.y);

        itemRegistry.SpawnItem(itemId, dropPosition, Quaternion.identity);
        // Instantiate(prefab, dropPosition, Quaternion.identity).ResetPhysics();
    }
}
