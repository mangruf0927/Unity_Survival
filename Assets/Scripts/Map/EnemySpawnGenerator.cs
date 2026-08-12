using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySpawnGenerator
{
    private readonly MapGrid mapGrid;
    private readonly Transform mapParent;
    private readonly EnemySpawner enemySpawner;
    private readonly List<LevelEnemySpawnInfo> levelSpawnInfoList;
    private readonly float cellSize;

    public EnemySpawnGenerator(MapGrid mapGrid, Transform mapParent, EnemySpawner enemySpawner, List<LevelEnemySpawnInfo> levelSpawnInfoList, float cellSize)
    {
        this.mapGrid = mapGrid;
        this.mapParent = mapParent;
        this.enemySpawner = enemySpawner;
        this.levelSpawnInfoList = levelSpawnInfoList;
        this.cellSize = cellSize;
    }

    public async UniTask GenerateAsync(System.Random random, CancellationToken ct)
    {
        if (levelSpawnInfoList == null || levelSpawnInfoList.Count == 0)
        {
            Debug.LogWarning("Level Enemy Spawn List is empty");
            return;
        }

        const int enemyPerFrame = 50;
        int counter = 0;

        foreach (LevelEnemySpawnInfo levelInfo in levelSpawnInfoList)
        {
            if (levelInfo == null || levelInfo.spawnEntryList == null) continue;

            GameObject spawnParent = new($"Lv{levelInfo.mapLevel}Spawner");
            spawnParent.transform.SetParent(mapParent);
            spawnParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (EnemySpawnEntry spawnEntry in levelInfo.spawnEntryList)
            {
                if (spawnEntry == null || spawnEntry.spawnCount <= 0) continue;

                List<CellData> availableCellList = mapGrid.GetAvailableCells(levelInfo.mapLevel);

                for (int i = 0; i < spawnEntry.spawnCount; i++)
                {
                    if (availableCellList.Count == 0)
                    {
                        Debug.LogWarning($"Not enough cells. Level: {levelInfo.mapLevel}");
                        break;
                    }

                    int randomIndex = random.Next(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[randomIndex];

                    GameObject spawnPointObject = CreateSpawnPoint(spawnEntry.prefab, spawnParent.transform);
                    SetSpawnPointTransform(spawnPointObject, selectedCell, spawnEntry.offsetY, random);

                    enemySpawner.RegisterSpawnPoint(spawnEntry.groupId, spawnEntry.enemyId, levelInfo.mapLevel, spawnEntry.spawnRadius, spawnPointObject.transform);

                    selectedCell.SetCenterType(CenterType.ENEMYSPAWN);
                    availableCellList.RemoveAt(randomIndex);

                    if (++counter % enemyPerFrame == 0)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, ct);
                    }
                }
            }
        }
    }

    private GameObject CreateSpawnPoint(GameObject prefab, Transform parent)
    {
        if (prefab == null)
        {
            GameObject spawnPoint = new();
            spawnPoint.transform.SetParent(parent);

            return spawnPoint;
        }

        return Object.Instantiate(prefab, parent);
    }

    private void SetSpawnPointTransform(GameObject spawnPointObject, CellData cell, float offsetY, System.Random random)
    {
        Vector2Int coordinate = cell.Coordinate;

        Vector3 position = new(coordinate.x * cellSize, cell.Height + offsetY, coordinate.y * cellSize);
        int rotation = random.Next(0, 4) * 90;
        spawnPointObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));

    }
}
