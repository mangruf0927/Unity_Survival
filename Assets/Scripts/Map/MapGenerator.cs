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
    private GroundGenerator groundGenerator;
    private EnemySpawnGenerator enemySpawnGenerator;
    private StructureGenerator structureGenerator;
    private ItemSpotGenerator itemSpotGenerator;
    private CancellationTokenSource cts;

    private void Awake()
    {
        mapGrid = new MapGrid(levelRadiusList);
        groundGenerator = new GroundGenerator(mapGrid, transform, groundPrefab, mapRadius,
                                              cellSize, cellThickness, noiseScale, heightStep, maxHeightStep);
        structureGenerator = new StructureGenerator(mapGrid, transform, structureSpawnEntryList, structureCountList, levelChestSpawnInfoList,
                                                    itemRegistry, objectRegistry, cellSize, cellThickness, heightStep);
        itemSpotGenerator = new ItemSpotGenerator(mapGrid, transform, itemSpotCountList, itemSpotPrefabList,
                                                  itemRegistry, objectRegistry, cellSize);
        enemySpawnGenerator = new EnemySpawnGenerator(mapGrid, transform, enemySpawner, levelEnemySpawnInfoList, cellSize);
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

        await groundGenerator.GenerateAsync(noiseOffsetX, noiseOffsetZ, ct);

        CreateCampFire();

        await structureGenerator.GenerateAsync(structureRandom, ct);
        await itemSpotGenerator.GenerateAsync(itemSpotRandom, ct);
        await enemySpawnGenerator.GenerateAsync(enemyRandom, ct);
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
