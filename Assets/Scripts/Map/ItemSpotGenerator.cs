using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ItemSpotGenerator
{
    private readonly MapGrid mapGrid;
    private readonly Transform mapParent;
    private readonly List<int> itemSpotCountList;
    private readonly List<ItemSpotSpawnEntry> itemSpotPrefabList;
    private readonly ItemRegistry itemRegistry;
    private readonly ObjectRegistry objectRegistry;
    private readonly float cellSize;

    public ItemSpotGenerator(MapGrid mapGrid, Transform mapParent, List<int> itemSpotCountList, List<ItemSpotSpawnEntry> itemSpotPrefabList,
        ItemRegistry itemRegistry, ObjectRegistry objectRegistry, float cellSize)
    {
        this.mapGrid = mapGrid;
        this.mapParent = mapParent;
        this.itemSpotCountList = itemSpotCountList;
        this.itemSpotPrefabList = itemSpotPrefabList;
        this.itemRegistry = itemRegistry;
        this.objectRegistry = objectRegistry;
        this.cellSize = cellSize;
    }

    public async UniTask GenerateAsync(System.Random random, CancellationToken ct)
    {
        GameObject itemSpotParent = new("ItemSpots");
        itemSpotParent.transform.SetParent(mapParent, false);

        const int itemSpotsPerFrame = 10;
        int counter = 0;

        for (int level = 1; level <= itemSpotCountList.Count; level++)
        {
            int spawnCount = itemSpotCountList[level - 1];
            if (spawnCount <= 0) continue;

            List<CellData> availableCellList = mapGrid.GetAvailableCells(level);

            for (int i = 0; i < spawnCount; i++)
            {
                if (availableCellList.Count == 0)
                {
                    Debug.LogWarning($"Level {level}: Not enough cells to place LootSpot");
                    break;
                }

                int index = random.Next(0, availableCellList.Count);
                CellData selectedCell = availableCellList[index];

                availableCellList.RemoveAt(index);

                int itemIndex = random.Next(0, itemSpotPrefabList.Count);
                ItemSpotSpawnEntry spawnEntry = itemSpotPrefabList[itemIndex];

                GameObject itemSpotObject = Object.Instantiate(spawnEntry.prefab, itemSpotParent.transform);

                Vector2Int coordinate = selectedCell.Coordinate;
                Vector3 position = new(coordinate.x * cellSize, selectedCell.Height + spawnEntry.offsetY, coordinate.y * cellSize);

                int rotation = random.Next(0, 4) * 90;
                itemSpotObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));

                if (!itemSpotObject.TryGetComponent(out ItemSpot itemSpot))
                {
                    Debug.LogWarning($"{itemSpotObject.name}: ItemSpot component not found.");
                    Object.Destroy(itemSpotObject);
                    continue;
                }

                objectRegistry.RegisterGeneratedObjects(itemSpotObject);
                selectedCell.SetCenterType(CenterType.ITEMSPOT);
                itemSpot.SpawnItem(level, random, itemRegistry);

                if (++counter % itemSpotsPerFrame == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
        }
    }
}
