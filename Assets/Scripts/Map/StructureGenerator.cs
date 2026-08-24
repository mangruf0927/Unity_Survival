using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StructureGenerator
{
    private MapGrid mapGrid;
    private Transform mapParent;

    private readonly List<StructureSpawnEntry> spawnEntryList;
    private readonly List<int> structureCountList;
    private readonly List<LevelChestSpawnInfo> levelChestSpawnInfoList;

    private readonly ItemRegistry itemRegistry;
    private readonly EquippableRegistry equippableRegistry;
    private readonly ObjectRegistry objectRegistry;

    private readonly float cellSize;
    private readonly float cellThickness;
    private readonly float heightStep;

    public StructureGenerator(MapGrid mapGrid, Transform mapParent,
        List<StructureSpawnEntry> spawnEntryList, List<int> structureCountList, List<LevelChestSpawnInfo> levelChestSpawnInfoList,
        ItemRegistry itemRegistry, EquippableRegistry equippableRegistry, ObjectRegistry objectRegistry,
        float cellSize, float cellThickness, float heightStep)
    {
        this.mapGrid = mapGrid;
        this.mapParent = mapParent;
        this.spawnEntryList = spawnEntryList;
        this.structureCountList = structureCountList;
        this.levelChestSpawnInfoList = levelChestSpawnInfoList;
        this.itemRegistry = itemRegistry;
        this.equippableRegistry = equippableRegistry;
        this.objectRegistry = objectRegistry;
        this.cellSize = cellSize;
        this.cellThickness = cellThickness;
        this.heightStep = heightStep;
    }

    public async UniTask GenerateAsync(System.Random random, CancellationToken ct)
    {
        GameObject structureParent = new("Structures");
        structureParent.transform.SetParent(mapParent);

        const int structuresPerFrame = 10;
        int counter = 0;

        for (int level = 1; level <= structureCountList.Count; level++)
        {
            int count = structureCountList[level - 1];

            for (int i = 0; i < count; i++)
            {
                bool placed = false;

                List<CellData> availableCellList = mapGrid.GetAvailableCells(level);
                if (availableCellList.Count == 0) break;

                for (int attempt = 0; attempt < 50; attempt++)
                {
                    int structureIndex = random.Next(0, spawnEntryList.Count);
                    StructureSpawnEntry spawnEntry = spawnEntryList[structureIndex];

                    if (spawnEntry == null || spawnEntry.prefab == null || spawnEntry.size.x <= 0 || spawnEntry.size.y <= 0) continue;

                    int cellIndex = random.Next(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[cellIndex];

                    List<CellData> structureCellList = mapGrid.GetStructureCells(selectedCell.Coordinate, spawnEntry.size, level);
                    if (structureCellList == null) continue;

                    PlaceStructure(level, spawnEntry, structureCellList, structureParent.transform, random);
                    placed = true;
                    break;
                }

                if (!placed)
                {
                    Debug.LogWarning($"Failed to place structure. Level: {level}");
                }

                if (++counter % structuresPerFrame == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
        }
    }

    private void PlaceStructure(int level, StructureSpawnEntry entry, List<CellData> cellList, Transform parent, System.Random random)
    {
        float totalHeight = 0f;
        Vector3 center = Vector3.zero;

        foreach (CellData cell in cellList)
        {
            totalHeight += cell.Height;
            center += new Vector3(cell.Coordinate.x * cellSize, 0f, cell.Coordinate.y * cellSize);
        }

        float averageHeight = totalHeight / cellList.Count;

        if (heightStep > 0f) averageHeight = Mathf.Round(averageHeight / heightStep) * heightStep;

        center /= cellList.Count;
        center.y = averageHeight + entry.offsetY;

        foreach (CellData cell in cellList)
        {
            cell.SetHeight(averageHeight, cellThickness);
            cell.SetCenterType(CenterType.STRUCTURE);
        }

        GameObject structureObj = Object.Instantiate(entry.prefab, parent);
        int rotation = random.Next(0, 4) * 90;

        structureObj.transform.SetLocalPositionAndRotation(center, Quaternion.Euler(0f, rotation, 0f));

        if (!structureObj.TryGetComponent(out Structure structure)) return;

        objectRegistry.RegisterGeneratedObjects(structureObj);
        structure.SpawnItems(random, itemRegistry);

        GameObject chestPrefab = GetRandomChest(level, random);
        structure.SpawnChests(random, chestPrefab, itemRegistry, equippableRegistry, objectRegistry);
    }

    private GameObject GetRandomChest(int level, System.Random random)
    {
        LevelChestSpawnInfo levelInfo = levelChestSpawnInfoList[level - 1];
        List<GameObject> validPrefabList = levelInfo.chestEntryList.FindAll(prefab => prefab != null);
        if (validPrefabList.Count == 0) return null;

        int index = random.Next(0, validPrefabList.Count);
        return validPrefabList[index];
    }

}
