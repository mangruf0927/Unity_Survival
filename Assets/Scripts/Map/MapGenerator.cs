using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

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
    [FormerlySerializedAs("levelRadius")]
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

    private readonly Dictionary<Vector2Int, CellData> cellDictionary = new();

    private void Awake()
    {
        InitializeSeed();

        GenerateGround();
        CreateCampFire();
        CreateStructures();
        CreateItemSpots();
        CreateEnemySpawns();
        CreateEnvironments();

        navMeshSurface.BuildNavMesh();
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

    // Ground
    private void GenerateGround()
    {
        if (groundPrefab == null)
        {
            Debug.LogWarning("Ground Prefab is null");
            return;
        }

        ClearGround();

        GameObject groundParent = new("Grounds");
        groundParent.transform.SetParent(transform);

        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int z = -mapRadius; z <= mapRadius; z++)
            {
                Vector2Int coordinate = new(x, z);

                if (!IsInsideRadius(coordinate, mapRadius)) continue;

                float height = GetCellHeight(coordinate);
                CreateCell(coordinate, height, groundParent.transform);
            }
        }
    }

    private void ClearGround()
    {
        foreach (CellData data in cellDictionary.Values)
        {
            if (data.GroundObject != null) Destroy(data.GroundObject);
        }
        cellDictionary.Clear();
    }

    private void CreateCell(Vector2Int coordinate, float height, Transform parent)
    {
        GameObject cell = Instantiate(groundPrefab, parent);

        cell.transform.localScale = new Vector3(cellSize, cellThickness, cellSize);
        cell.transform.localPosition = new Vector3(coordinate.x * cellSize, height - cellThickness * 0.5f, coordinate.y * cellSize);

        CellData cellData = new(coordinate, height, cell);
        cellDictionary.Add(coordinate, cellData);
    }

    private float GetCellHeight(Vector2Int coordinate)
    {
        if (IsCampFireArea(coordinate)) return 0f;

        float sampleX = coordinate.x * noiseScale + noiseOffsetX;
        float sampleZ = coordinate.y * noiseScale + noiseOffsetZ;

        float noise = Mathf.PerlinNoise(sampleX, sampleZ);
        float centeredNoise = noise * 2f - 1f;

        int step = Mathf.RoundToInt(centeredNoise * maxHeightStep);

        return step * heightStep;
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

        if (!cellDictionary.TryGetValue(coordinate, out CellData cell))
        {
            Debug.LogWarning("Cell (0, 0) could not be found");
            return;
        }

        campFire.transform.SetParent(transform);
        campFire.transform.localPosition = new Vector3(coordinate.x * cellSize, cell.Height + campFireY, coordinate.y * cellSize);

        cell.SetCenterType(CenterType.CAMPFIRE);
    }

    // Structure
    private void CreateStructures()
    {
        GameObject structureParent = new("Structures");
        structureParent.transform.SetParent(transform);

        for (int level = 1; level <= structureCountList.Count; level++)
        {
            int count = structureCountList[level - 1];

            for (int i = 0; i < count; i++)
            {
                bool placed = false;

                for (int attempt = 0; attempt < 50; attempt++)
                {
                    int structureIndex = structureRandom.Next(0, structureSpawnEntryList.Count);
                    StructureSpawnEntry spawnEntry = structureSpawnEntryList[structureIndex];

                    if (spawnEntry == null || spawnEntry.prefab == null || spawnEntry.size.x <= 0 || spawnEntry.size.y <= 0) continue;

                    List<CellData> availableCellList = GetAvailableCellList(level);
                    if (availableCellList.Count == 0) break;

                    int cellIndex = structureRandom.Next(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[cellIndex];

                    List<CellData> structureCellList = GetStructureCellList(selectedCell.Coordinate, spawnEntry.size, level);
                    if (structureCellList == null) continue;

                    PlaceStructure(level, spawnEntry, structureCellList, structureParent.transform);
                    placed = true;
                    break;
                }

                if (!placed)
                {
                    Debug.LogWarning($"Failed to place structure. Level: {level}");
                }
            }
        }
    }

    private List<CellData> GetStructureCellList(Vector2Int start, Vector2Int size, int level)
    {
        List<CellData> cellList = new();

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                Vector2Int coordinate = new(start.x + x, start.y + z);

                if (!cellDictionary.TryGetValue(coordinate, out CellData cell)) return null;
                if (cell.Type != CenterType.NONE) return null;
                if (!IsCellInLevel(coordinate, level)) return null;

                cellList.Add(cell);
            }
        }
        return cellList;
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
    private void CreateItemSpots()
    {
        GameObject itemSpotParent = new("ItemSpots");
        itemSpotParent.transform.SetParent(transform, false);

        for (int level = 1; level <= itemSpotCountList.Count; level++)
        {
            int spawnCount = itemSpotCountList[level - 1];
            if (spawnCount <= 0) continue;

            List<CellData> availableCellList = GetAvailableCellList(level);

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
    private void CreateEnemySpawns()
    {
        if (levelEnemySpawnInfoList == null || levelEnemySpawnInfoList.Count == 0)
        {
            Debug.LogWarning("Level Enemy Spawn List is empty");
            return;
        }

        foreach (LevelEnemySpawnInfo levelInfo in levelEnemySpawnInfoList)
        {
            if (levelInfo == null || levelInfo.spawnEntryList == null) continue;

            GameObject spawnParent = new($"Lv{levelInfo.mapLevel}Spawner");
            spawnParent.transform.SetParent(transform);
            spawnParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (EnemySpawnEntry spawnEntry in levelInfo.spawnEntryList)
            {
                if (spawnEntry == null || spawnEntry.spawnCount <= 0) continue;

                List<CellData> availableCellList = GetAvailableCellList(levelInfo.mapLevel);

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
                }
            }
        }
    }

    // Environment
    private void CreateEnvironments()
    {
        List<CellData> availableCellList = GetEnvironmentCellList();
        if (availableCellList.Count == 0) return;

        GameObject environmentParent = new("Environments");
        environmentParent.transform.SetParent(transform, false);

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
            }
        }
    }

    private List<CellData> GetEnvironmentCellList()
    {
        List<CellData> availableCellList = new();

        foreach (CellData cell in cellDictionary.Values)
        {
            if (cell.Type == CenterType.STRUCTURE) continue;
            if (IsCampFireArea(cell.Coordinate)) continue;

            availableCellList.Add(cell);
        }

        availableCellList.Sort((a, b) =>
        {
            int xCompare = a.Coordinate.x.CompareTo(b.Coordinate.x);
            return xCompare != 0 ? xCompare : a.Coordinate.y.CompareTo(b.Coordinate.y);
        });

        return availableCellList;
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

    // >> 
    private List<CellData> GetAvailableCellList(int level)
    {
        List<CellData> availableCellList = new();

        foreach (CellData cell in cellDictionary.Values)
        {
            if (cell.Type != CenterType.NONE) continue;
            if (IsCampFireArea(cell.Coordinate)) continue;
            if (!IsCellInLevel(cell.Coordinate, level)) continue;

            availableCellList.Add(cell);
        }

        availableCellList.Sort((a, b) =>
        {
            int xCompare = a.Coordinate.x.CompareTo(b.Coordinate.x);
            return xCompare != 0 ? xCompare : a.Coordinate.y.CompareTo(b.Coordinate.y);
        });

        return availableCellList;
    }

    private bool IsCellInLevel(Vector2Int coordinate, int level)
    {
        int outerRadius = GetLevelRadius(level);

        if (outerRadius < 0) return false;
        if (!IsInsideRadius(coordinate, outerRadius)) return false;
        if (level == 1) return true;

        int innerRadius = GetLevelRadius(level - 1);

        return !IsInsideRadius(coordinate, innerRadius);
    }

    private bool IsCampFireArea(Vector2Int coordinate)
    {
        return Mathf.Abs(coordinate.x) <= 1 && Mathf.Abs(coordinate.y) <= 1;
    }

    private int GetLevelRadius(int level)
    {
        if (levelRadiusList == null) return -1;
        if (level < 1 || level > levelRadiusList.Count) return -1;

        return levelRadiusList[level - 1];
    }

    private bool IsInsideRadius(Vector2Int coordinate, int radius)
    {
        return coordinate.sqrMagnitude < radius * radius;
    }
}
