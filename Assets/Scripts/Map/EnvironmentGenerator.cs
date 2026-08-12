using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnvironmentGenerator
{
    private readonly MapGrid mapGrid;
    private readonly Transform mapParent;
    private readonly List<EnvironmentSpawnEntry> spawnEntryList;
    private readonly ItemRegistry itemRegistry;
    private readonly ObjectRegistry objectRegistry;
    private readonly float cellSize;


    public EnvironmentGenerator(MapGrid mapGrid, Transform mapParent, List<EnvironmentSpawnEntry> spawnEntryList,
        ItemRegistry itemRegistry, ObjectRegistry objectRegistry, float cellSize)
    {
        this.mapGrid = mapGrid;
        this.mapParent = mapParent;
        this.spawnEntryList = spawnEntryList;
        this.itemRegistry = itemRegistry;
        this.objectRegistry = objectRegistry;
        this.cellSize = cellSize;
    }


    public async UniTask GenerateAsync(System.Random random, CancellationToken ct)
    {
        List<CellData> availableCellList = mapGrid.GetEnvironmentCells();
        if (availableCellList.Count == 0) return;

        GameObject environmentParent = new("Environments");
        environmentParent.transform.SetParent(mapParent, false);

        const int environmentPerFrame = 50;
        int counter = 0;

        foreach (EnvironmentSpawnEntry entry in spawnEntryList)
        {
            int minCount = Mathf.Max(0, entry.minSpawnCount);
            int maxCount = Mathf.Max(minCount, entry.maxSpawnCount);
            int maxCountPerCell = Mathf.Max(1, entry.maxCountPerCell);
            int spawnCount = random.Next(minCount, maxCount + 1);

            List<CellData> entryCellList = new(availableCellList);
            Dictionary<Vector2Int, int> cellCountMap = new();

            for (int i = 0; i < spawnCount; i++)
            {
                if (entryCellList.Count == 0) break;

                int cellIndex = random.Next(0, entryCellList.Count);
                CellData selectedCell = entryCellList[cellIndex];

                GameObject environment = Object.Instantiate(entry.prefab, environmentParent.transform);
                WorldObject worldObject = environment.GetComponentInChildren<WorldObject>();

                if (worldObject != null)
                {
                    worldObject.Initialize(itemRegistry);
                    objectRegistry.RegisterGenerated(worldObject);
                }

                Vector3 position = GetRandomPositionInCell(random, selectedCell);
                position.y += entry.offsetY;

                int rotation = random.Next(0, 4) * 90;
                environment.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));

                cellCountMap.TryGetValue(selectedCell.Coordinate, out int currentCount);
                currentCount++;

                cellCountMap[selectedCell.Coordinate] = currentCount;

                if (currentCount >= maxCountPerCell)
                {
                    entryCellList.RemoveAt(cellIndex);
                }

                if (++counter % environmentPerFrame == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
        }
    }

    private Vector3 GetRandomPositionInCell(System.Random random, CellData cell)
    {
        float range = cellSize * 0.4f;
        float offsetX = Mathf.Lerp(-range, range, (float)random.NextDouble());
        float offsetZ = Mathf.Lerp(-range, range, (float)random.NextDouble());

        float x = cell.Coordinate.x * cellSize + offsetX;
        float z = cell.Coordinate.y * cellSize + offsetZ;

        return new Vector3(x, cell.Height, z);
    }
}
