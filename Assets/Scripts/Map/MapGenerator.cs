using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class StructureSpawnEntry
{
    public GameObject prefab;
    public Vector2Int size = Vector2Int.one;
    public float offsetY;
}

[System.Serializable]
public class EnemySpawnEntry
{
    public int groupId;
    public int enemyId;
    public int spawnCount;
    public float spawnRadius;
    public GameObject prefab;
    public float offsetY;
}

[System.Serializable]
public class ItemSpotSpawnEntry
{
    public GameObject prefab;
    public float offsetY;
}

[System.Serializable]
public class LevelEnemySpawnInfo
{
    public int mapLevel;
    public List<EnemySpawnEntry> spawnEntryList;
}

[System.Serializable]
public class LevelChestSpawnInfo
{
    public int mapLevel;
    public List<GameObject> chestEntryList;
}

[System.Serializable]
public class EnvironmentSpawnEntry
{
    public GameObject prefab;
    public int minSpawnCount;
    public int maxSpawnCount;
    public int maxCountPerCell;
    public float offsetY;
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    [SerializeField] private bool useSeed;

    [SerializeField] private float noiseScale;
    [SerializeField] private float heightStep;
    [SerializeField] private int maxHeightStep;

    [SerializeField] private int mapRadius;
    [SerializeField] private List<int> levelRadiusList;

    [Header("땅")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private float cellSize;
    [SerializeField] private float cellThickness;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("캠프파이어")]
    [SerializeField] private CampFire campFire;
    [SerializeField] private float campFireY;

    [Header("구조물")]
    [FormerlySerializedAs("structureEntryList")]
    [SerializeField] private List<StructureSpawnEntry> structureSpawnEntryList;
    [SerializeField] private List<int> structureCountList;

    [Header("아이템")]
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private ObjectRegistry objectRegistry;
    [SerializeField] private List<LevelChestSpawnInfo> levelChestSpawnInfoList;

    [Header("아이템 생성소")]
    [SerializeField] private List<int> itemSpotCountList;
    [SerializeField] private List<ItemSpotSpawnEntry> itemSpotPrefabList;

    [Header("Enemy")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<LevelEnemySpawnInfo> levelEnemySpawnInfoList;

    [Header("환경 오브젝트")]
    [SerializeField] private List<EnvironmentSpawnEntry> environmentSpawnEntryList;

    private float noiseOffsetX;
    private float noiseOffsetZ;

    private System.Random structureRandom;
    private System.Random itemSpotRandom;
    private System.Random enemyRandom;
    private System.Random environmentRandom;

    private MapGrid mapGrid;
    private MapGroundGenerator groundGenerator;
    private CancellationTokenSource cts;

    private void Awake()
    {
        mapGrid = new MapGrid(levelRadiusList);
        groundGenerator = new MapGroundGenerator(mapGrid, transform, groundPrefab, mapRadius,
                                cellSize, cellThickness, noiseScale, heightStep, maxHeightStep);
    }

    private void Start()
    {
        cts = new CancellationTokenSource();
        GenerateMapAsync(cts.Token).Forget();
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTask GenerateMapAsync(CancellationToken ct)
    {
        InitializeSeed();

        await groundGenerator.GenerateGroundAsync(noiseOffsetX, noiseOffsetZ, ct);

        CreateCampFire();

        await CreateStructures(ct);
        await CreateItemSpots(ct);
        await CreateEnemySpawns(ct);
        await CreateEnvironments(ct);

        bool isNavMeshReady = await BuildNavMeshAsync(ct);
        if (isNavMeshReady)
        {
            enemySpawner.Initialize();
        }

    }

    private async UniTask<bool> BuildNavMeshAsync(CancellationToken ct)
    {
        if (navMeshSurface == null)
        {
            Debug.LogWarning("NavMeshSurface is null");
            return false;
        }

        if (navMeshSurface.navMeshData == null)
        {
            navMeshSurface.BuildNavMesh(); // 최초 1회 초기화
            return true;
        }

        await navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData).ToUniTask(cancellationToken: ct);
        return true;
    }


    private void InitializeSeed()
    {
        if (!useSeed) seed = Random.Range(1, 1000000);

        Random.InitState(seed);

        noiseOffsetX = Random.Range(-100000f, 100000f);
        noiseOffsetZ = Random.Range(-100000f, 100000f);

        structureRandom = new System.Random(seed + 1);
        itemSpotRandom = new System.Random(seed + 2);
        enemyRandom = new System.Random(seed + 3);
        environmentRandom = new System.Random(seed + 4);
    }

    // Campfire
    private void CreateCampFire()
    {
        if (campFire == null)
        {
            Debug.LogWarning("CampFire is null");
            return;
        }

        Vector2Int coordinate = Vector2Int.zero;

        if (!mapGrid.TryGetCell(coordinate, out CellData cell))
        {
            Debug.LogWarning("Cell (0, 0) could not be found");
            return;
        }

        campFire.transform.SetParent(transform);
        campFire.transform.localPosition = new Vector3(coordinate.x * cellSize, cell.Height + campFireY, coordinate.y * cellSize);

        cell.SetCenterType(CenterType.CAMPFIRE);
    }

    // Structure
    private async UniTask CreateStructures(CancellationToken ct)
    {
        GameObject structureParent = new("Structures");
        structureParent.transform.SetParent(transform);

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
                    int structureIndex = structureRandom.Next(0, structureSpawnEntryList.Count);
                    StructureSpawnEntry spawnEntry = structureSpawnEntryList[structureIndex];

                    if (spawnEntry == null || spawnEntry.prefab == null || spawnEntry.size.x <= 0 || spawnEntry.size.y <= 0) continue;

                    int cellIndex = structureRandom.Next(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[cellIndex];

                    List<CellData> structureCellList = mapGrid.GetStructureCells(selectedCell.Coordinate, spawnEntry.size, level);
                    if (structureCellList == null) continue;

                    PlaceStructure(level, spawnEntry, structureCellList, structureParent.transform);
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

    private void PlaceStructure(int level, StructureSpawnEntry entry, List<CellData> cellList, Transform structureParent)
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

        GameObject structureObj = Instantiate(entry.prefab, structureParent);
        int rotation = structureRandom.Next(0, 4) * 90;

        structureObj.transform.SetLocalPositionAndRotation(center, Quaternion.Euler(0f, rotation, 0f));

        if (!structureObj.TryGetComponent(out Structure structure)) return;

        RegisterDoors(structureObj);
        structure.SpawnItems(structureRandom, itemRegistry);

        GameObject chestPrefab = GetRandomChest(level);
        structure.SpawnChests(structureRandom, chestPrefab, itemRegistry, objectRegistry);
    }

    private GameObject GetRandomChest(int level)
    {
        LevelChestSpawnInfo levelInfo = levelChestSpawnInfoList[level - 1];
        List<GameObject> validPrefabList = levelInfo.chestEntryList.FindAll(prefab => prefab != null);
        if (validPrefabList.Count == 0) return null;

        int index = structureRandom.Next(0, validPrefabList.Count);

        return validPrefabList[index];
    }

    // ItemSpot
    private async UniTask CreateItemSpots(CancellationToken ct)
    {
        GameObject itemSpotParent = new("ItemSpots");
        itemSpotParent.transform.SetParent(transform, false);

        const int itemSpotPerFrame = 10;
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

                int index = itemSpotRandom.Next(0, availableCellList.Count);
                CellData selectedCell = availableCellList[index];

                availableCellList.RemoveAt(index);

                int itemIndex = itemSpotRandom.Next(0, itemSpotPrefabList.Count);
                ItemSpotSpawnEntry spawnEntry = itemSpotPrefabList[itemIndex];

                GameObject itemSpotObject = Instantiate(spawnEntry.prefab, itemSpotParent.transform);

                Vector2Int coordinate = selectedCell.Coordinate;
                Vector3 position = new(coordinate.x * cellSize, selectedCell.Height + spawnEntry.offsetY, coordinate.y * cellSize);
                int rotation = itemSpotRandom.Next(0, 4) * 90;
                itemSpotObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));

                if (!itemSpotObject.TryGetComponent(out ItemSpot itemSpot))
                {
                    Debug.LogWarning($"{itemSpotObject.name}: ItemSpot component not found.");
                    Destroy(itemSpotObject);
                    continue;
                }

                RegisterDoors(itemSpotObject);
                selectedCell.SetCenterType(CenterType.ITEMSPOT);
                itemSpot.SpawnItem(level, itemSpotRandom, itemRegistry);

                if (++counter % itemSpotPerFrame == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
        }
    }

    private void RegisterDoors(GameObject obj)
    {
        Door[] doors = obj.GetComponentsInChildren<Door>(true);

        foreach (Door door in doors)
        {
            if (door == null) continue;
            objectRegistry.RegisterGenerated(door);
        }
    }

    // Enemy 
    private async UniTask CreateEnemySpawns(CancellationToken ct)
    {
        if (levelEnemySpawnInfoList == null || levelEnemySpawnInfoList.Count == 0)
        {
            Debug.LogWarning("Level Enemy Spawn List is empty");
            return;
        }

        const int enemyPerFrame = 50;
        int counter = 0;

        foreach (LevelEnemySpawnInfo levelInfo in levelEnemySpawnInfoList)
        {
            if (levelInfo == null || levelInfo.spawnEntryList == null) continue;

            GameObject spawnParent = new($"Lv{levelInfo.mapLevel}Spawner");
            spawnParent.transform.SetParent(transform);
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

                    int randomIndex = enemyRandom.Next(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[randomIndex];
                    Vector2Int coordinate = selectedCell.Coordinate;

                    GameObject spawnPointObject;

                    if (spawnEntry.prefab == null)
                    {
                        spawnPointObject = new GameObject();
                        spawnPointObject.transform.SetParent(spawnParent.transform);
                    }
                    else
                    {
                        spawnPointObject = Instantiate(spawnEntry.prefab, spawnParent.transform);
                    }

                    Vector3 position = new(coordinate.x * cellSize, selectedCell.Height + spawnEntry.offsetY, coordinate.y * cellSize);
                    int rotation = enemyRandom.Next(0, 4) * 90;
                    spawnPointObject.transform.SetLocalPositionAndRotation(position, Quaternion.Euler(0f, rotation, 0f));

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

    // Environment
    private async UniTask CreateEnvironments(CancellationToken ct)
    {
        List<CellData> availableCellList = mapGrid.GetEnvironmentCells();
        if (availableCellList.Count == 0) return;

        GameObject environmentParent = new("Environments");
        environmentParent.transform.SetParent(transform, false);

        const int environmentPerFrame = 50;
        int counter = 0;

        foreach (EnvironmentSpawnEntry entry in environmentSpawnEntryList)
        {
            int minCount = Mathf.Max(0, entry.minSpawnCount);
            int maxCount = Mathf.Max(minCount, entry.maxSpawnCount);
            int maxCountPerCell = Mathf.Max(1, entry.maxCountPerCell);
            int spawnCount = environmentRandom.Next(minCount, maxCount + 1);

            List<CellData> entryCellList = new(availableCellList);
            Dictionary<Vector2Int, int> cellCountMap = new();

            for (int i = 0; i < spawnCount; i++)
            {
                if (entryCellList.Count == 0) break;

                int cellIndex = environmentRandom.Next(0, entryCellList.Count);
                CellData selectedCell = entryCellList[cellIndex];

                GameObject environment = Instantiate(entry.prefab, environmentParent.transform);
                WorldObject worldObject = environment.GetComponentInChildren<WorldObject>();

                if (worldObject != null)
                {
                    worldObject.Initialize(itemRegistry);
                    objectRegistry.RegisterGenerated(worldObject);
                }

                Vector3 position = GetRandomPositionInCell(selectedCell);
                position.y += entry.offsetY;

                int rotation = environmentRandom.Next(0, 4) * 90;
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


    private Vector3 GetRandomPositionInCell(CellData cell)
    {
        float range = cellSize * 0.4f;
        float offsetX = Mathf.Lerp(-range, range, (float)environmentRandom.NextDouble());
        float offsetZ = Mathf.Lerp(-range, range, (float)environmentRandom.NextDouble());

        float x = cell.Coordinate.x * cellSize + offsetX;
        float z = cell.Coordinate.y * cellSize + offsetZ;

        return new Vector3(x, cell.Height, z);
    }
}
