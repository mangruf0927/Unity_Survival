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
public class LevelEnemySpawnInfo
{
    public int mapLevel;
    public List<EnemySpawnEntry> spawnEntryList;
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

    [Header("Enemy")]
    [SerializeField] private EnemySpawner enemySpawner;
    [FormerlySerializedAs("levelEnemySpawnList")]
    [SerializeField] private List<LevelEnemySpawnInfo> levelEnemySpawnInfoList;

    private float noiseOffsetX;
    private float noiseOffsetZ;

    private System.Random structureRandom;
    private System.Random enemyRandom;

    private readonly Dictionary<Vector2Int, CellData> cellDictionary = new();

    private void Awake()
    {
        InitializeSeed();

        GenerateGround();
        CreateCampFire();
        CreateStructures();
        CreateEnemySpawns();
    }

    private void InitializeSeed()
    {
        if (!useSeed) seed = Random.Range(1, 1000000);

        Random.InitState(seed);

        noiseOffsetX = Random.Range(-100000f, 100000f);
        noiseOffsetZ = Random.Range(-100000f, 100000f);

        structureRandom = new System.Random(seed + 1);
        enemyRandom = new System.Random(seed + 2);
    }

    private void GenerateGround()
    {
        if (groundPrefab == null)
        {
            Debug.LogWarning("Ground Prefab is null");
            return;
        }

        ClearGround();

        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int z = -mapRadius; z <= mapRadius; z++)
            {
                Vector2Int coordinate = new(x, z);

                if (!IsInsideRadius(coordinate, mapRadius)) continue;

                float height = GetCellHeight(coordinate);
                CreateCell(coordinate, height);
            }
        }
        navMeshSurface.BuildNavMesh();
    }

    private void ClearGround()
    {
        foreach (CellData data in cellDictionary.Values)
        {
            if (data.GroundObject != null) Destroy(data.GroundObject);
        }

        cellDictionary.Clear();
    }

    private void CreateCell(Vector2Int coordinate, float height)
    {
        GameObject cell = Instantiate(groundPrefab, transform);

        cell.transform.localScale = new Vector3(cellSize, cellThickness, cellSize);
        cell.transform.localPosition = new Vector3(coordinate.x * cellSize, height - cellThickness * 0.5f, coordinate.y * cellSize);

        CellData cellData = new(coordinate, height, cell);
        cellDictionary.Add(coordinate, cellData);
    }

    private float GetCellHeight(Vector2Int coordinate)
    {
        bool isCampFireArea = Mathf.Abs(coordinate.x) <= 1 && Mathf.Abs(coordinate.y) <= 1;

        if (isCampFireArea) return 0f;

        float sampleX = coordinate.x * noiseScale + noiseOffsetX;
        float sampleZ = coordinate.y * noiseScale + noiseOffsetZ;

        float noise = Mathf.PerlinNoise(sampleX, sampleZ);
        float centeredNoise = noise * 2f - 1f;

        int step = Mathf.RoundToInt(centeredNoise * maxHeightStep);

        return step * heightStep;
    }

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

                    PlaceStructure(spawnEntry, structureCellList, structureParent.transform);
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

    private void PlaceStructure(StructureSpawnEntry entry, List<CellData> cellList, Transform structureParent)
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

        GameObject structure = Instantiate(entry.prefab, structureParent);
        int rotation = structureRandom.Next(0, 4) * 90;

        structure.transform.SetLocalPositionAndRotation(center, Quaternion.Euler(0f, rotation, 0f));
    }

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

                    spawnPointObject.transform.localPosition = new Vector3(coordinate.x * cellSize, selectedCell.Height + spawnEntry.offsetY, coordinate.y * cellSize);

                    int randomRotation = enemyRandom.Next(0, 4) * 90;
                    spawnPointObject.transform.localRotation = Quaternion.Euler(0f, randomRotation, 0f);

                    enemySpawner.RegisterSpawnPoint(spawnEntry.groupId, spawnEntry.enemyId, levelInfo.mapLevel, spawnEntry.spawnRadius, spawnPointObject.transform);

                    selectedCell.SetCenterType(CenterType.ENEMYSPAWN);
                    availableCellList.RemoveAt(randomIndex);
                }
            }
        }
    }

    private List<CellData> GetAvailableCellList(int level)
    {
        List<CellData> availableCellList = new();

        foreach (CellData cell in cellDictionary.Values)
        {
            if (cell.Type != CenterType.NONE) continue;
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

    private int GetLevelRadius(int level)
    {
        if (levelRadiusList == null) return -1;
        if (level < 1 || level > levelRadiusList.Count) return -1;

        return levelRadiusList[level - 1] - 1;
    }

    private bool IsInsideRadius(Vector2Int coordinate, int radius)
    {
        float roundedRadius = radius + 0.5f;

        return coordinate.sqrMagnitude <= roundedRadius * roundedRadius;
    }
}
