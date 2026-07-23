using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public int enemyCount;
    public GameObject prefab;
    public float offsetY;
}

[System.Serializable]
public class LevelEnemySpawnInfo
{
    public int mapLevel;
    public List<EnemySpawnEntry> spawnList;
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    [SerializeField] private bool useSeed;

    [SerializeField] private float noiseScale;
    [SerializeField] private float heightStep;
    [SerializeField] private int maxHeightStep;

    [SerializeField] private int mapRadius;
    [SerializeField] private List<int> levelRadius;

    [Header("땅")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private float cellSize;
    [SerializeField] private float cellThickness;

    [Header("캠프파이어")]
    [SerializeField] private GameObject campFirePrefab;
    [SerializeField] private float campFireY;

    [Header("Enemy Spawn")]
    [SerializeField] private List<LevelEnemySpawnInfo> levelEnemySpawnList;

    private float noiseOffsetX;
    private float noiseOffsetZ;

    private readonly Dictionary<Vector2Int, CellData> cellDictionary = new();

    private void Start()
    {
        InitializeSeed();

        GenerateGround();
        CreateCampFire();
        CreateEnemySpawns();
    }

    private void InitializeSeed()
    {
        if (!useSeed) seed = Random.Range(1, 1000000);

        Random.InitState(seed);

        noiseOffsetX = Random.Range(-100000f, 100000f);
        noiseOffsetZ = Random.Range(-100000f, 100000f);
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
        if (campFirePrefab == null)
        {
            Debug.LogWarning("CampFire Prefab is null");
            return;
        }

        Vector2Int campFireCoordinate = Vector2Int.zero;

        if (!cellDictionary.TryGetValue(campFireCoordinate, out CellData cell))
        {
            Debug.LogWarning("Cell (0, 0) could not be found");
            return;
        }

        GameObject campFire = Instantiate(campFirePrefab, transform);
        campFire.transform.localPosition = new Vector3(campFireCoordinate.x * cellSize, cell.Height + campFireY, campFireCoordinate.y * cellSize);

        cell.SetCenterType(CenterType.CAMPFIRE);
    }

    private void CreateEnemySpawns()
    {
        if (levelEnemySpawnList == null || levelEnemySpawnList.Count == 0)
        {
            Debug.LogWarning("Level Enemy Spawn List is empty");
            return;
        }

        foreach (LevelEnemySpawnInfo levelInfo in levelEnemySpawnList)
        {
            if (levelInfo == null || levelInfo.spawnList == null) continue;

            GameObject levelParent = new($"Lv{levelInfo.mapLevel}Spawner");
            levelParent.transform.SetParent(transform);
            levelParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            foreach (EnemySpawnEntry spawnEntry in levelInfo.spawnList)
            {
                if (spawnEntry == null || spawnEntry.enemyCount <= 0) continue;

                List<CellData> availableCellList = GetAvailableCellList(levelInfo.mapLevel);

                for (int i = 0; i < spawnEntry.enemyCount; i++)
                {
                    if (availableCellList.Count == 0)
                    {
                        Debug.LogWarning($"Not enough cells. Level: {levelInfo.mapLevel}");
                        break;
                    }

                    int randomIndex = Random.Range(0, availableCellList.Count);
                    CellData selectedCell = availableCellList[randomIndex];
                    Vector2Int coordinate = selectedCell.Coordinate;

                    GameObject spawnObject;

                    if (spawnEntry.prefab == null)
                    {
                        spawnObject = new GameObject();
                        spawnObject.transform.SetParent(levelParent.transform);
                    }
                    else
                    {
                        spawnObject = Instantiate(spawnEntry.prefab, levelParent.transform);
                    }

                    spawnObject.transform.localPosition = new Vector3(coordinate.x * cellSize, selectedCell.Height + spawnEntry.offsetY, coordinate.y * cellSize);

                    int randomRotation = Random.Range(0, 4) * 90;
                    spawnObject.transform.localRotation = Quaternion.Euler(0f, randomRotation, 0f);

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
        if (levelRadius == null) return -1;
        if (level < 1 || level > levelRadius.Count) return -1;

        return levelRadius[level - 1] - 1;
    }

    private bool IsInsideRadius(Vector2Int coordinate, int radius)
    {
        float roundedRadius = radius + 0.5f;

        return coordinate.sqrMagnitude <= roundedRadius * roundedRadius;
    }
}