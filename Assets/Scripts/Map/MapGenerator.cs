using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
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
public class ItemSpotSpawnEntry
{
    public GameObject prefab;
    public float offsetY;
}

[System.Serializable]
public class LevelChestSpawnInfo
{
    public int mapLevel;
    public List<GameObject> chestEntryList;
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
    [SerializeField] private List<StructureSpawnEntry> structureSpawnEntryList;
    [SerializeField] private List<int> structureCountList;

    [Header("아이템")]
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EquippableRegistry equippableRegistry;
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
    private EnvironmentGenerator environmentGenerator;
    private NavMeshGenerator navMeshGenerator;
    private CancellationTokenSource cts;

    private void Awake()
    {
        mapGrid = new MapGrid(levelRadiusList);
        groundGenerator = new GroundGenerator(mapGrid, transform, groundPrefab, mapRadius,
                                              cellSize, cellThickness, noiseScale, heightStep, maxHeightStep);
        structureGenerator = new StructureGenerator(mapGrid, transform, structureSpawnEntryList, structureCountList, levelChestSpawnInfoList,
                                                    itemRegistry, equippableRegistry, objectRegistry, cellSize, cellThickness, heightStep);
        itemSpotGenerator = new ItemSpotGenerator(mapGrid, transform, itemSpotCountList, itemSpotPrefabList,
                                                  itemRegistry, objectRegistry, cellSize);
        enemySpawnGenerator = new EnemySpawnGenerator(mapGrid, transform, enemySpawner, levelEnemySpawnInfoList, cellSize);
        environmentGenerator = new EnvironmentGenerator(mapGrid, transform, environmentSpawnEntryList, itemRegistry, objectRegistry, cellSize);
        navMeshGenerator = new NavMeshGenerator(navMeshSurface);
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
        await environmentGenerator.GenerateAsync(environmentRandom, ct);

        bool isReady = await navMeshGenerator.GenerateAsync(ct);
        if (isReady) enemySpawner.Initialize();
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
}
