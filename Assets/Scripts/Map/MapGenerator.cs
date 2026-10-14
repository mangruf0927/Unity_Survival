using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class StructureSpawnEntry
{
    public AssetReferenceGameObject prefabReference;
    public Vector2Int size = Vector2Int.one;
    public float offsetY;

    [System.NonSerialized]
    public GameObject prefab;
}

[System.Serializable]
public class ItemSpotSpawnEntry
{
    public AssetReferenceGameObject prefabReference;
    public float offsetY;

    [System.NonSerialized] public GameObject prefab;
}

[System.Serializable]
public class LevelChestSpawnInfo
{
    public int mapLevel;
    public List<AssetReferenceGameObject> chestReferences;

    [System.NonSerialized] public List<GameObject> chestEntryList = new();
}

[System.Serializable]
public class EnemySpawnEntry
{
    public int groupId;
    public int enemyId;
    public int spawnCount;
    public float spawnRadius;
    public AssetReferenceGameObject prefabReference;
    public float offsetY;

    [System.NonSerialized] public GameObject prefab;
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
    public AssetReferenceGameObject prefabReference;
    public int minSpawnCount;
    public int maxSpawnCount;
    public int maxCountPerCell;
    public float offsetY;

    [System.NonSerialized] public GameObject prefab;
}

public class MapGenerator : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform player;

    [SerializeField] private int seed;
    [SerializeField] private bool useSeed;

    [SerializeField] private float noiseScale;
    [SerializeField] private float heightStep;
    [SerializeField] private int maxHeightStep;

    [SerializeField] private int mapRadius;
    [SerializeField] private List<int> levelRadiusList;

    [Header("땅")]
    [SerializeField] private AssetReferenceGameObject groundReference;
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

    private GameObject groundPrefab;
    private AddressableLoader addressableLoader;

    private void Awake()
    {
        if (player != null) player.gameObject.SetActive(false);

        addressableLoader = new AddressableLoader();
        mapGrid = new MapGrid(levelRadiusList);
        navMeshGenerator = new NavMeshGenerator(navMeshSurface);
    }

    private void OnDestroy()
    {
        addressableLoader?.ReleaseAll();
    }

    private async UniTask<bool> InitializeAsync(CancellationToken ct)
    {
        var groundTask = LoadGroundAsync(ct);
        var structureTask = LoadStructuresAsync(ct);
        var chestTask = LoadChestsAsync(ct);
        var itemSpotTask = LoadItemSpotsAsync(ct);
        var enemyTask = LoadEnemiesAsync(ct);
        var environmentTask = LoadEnvironmentsAsync(ct);

        var (ground, structure, chest, itemSpot, enemy, environment) = await UniTask.WhenAll(groundTask, structureTask, chestTask, itemSpotTask, enemyTask, environmentTask);
        if (!ground || !structure || !chest || !itemSpot || !enemy || !environment) return false;

        groundGenerator = new GroundGenerator(mapGrid, transform, groundPrefab, mapRadius, cellSize, cellThickness, noiseScale, heightStep, maxHeightStep);
        structureGenerator = new StructureGenerator(mapGrid, transform, structureSpawnEntryList, structureCountList, levelChestSpawnInfoList, itemRegistry, equippableRegistry, objectRegistry, cellSize, cellThickness, heightStep);
        itemSpotGenerator = new ItemSpotGenerator(mapGrid, transform, itemSpotCountList, itemSpotPrefabList, itemRegistry, objectRegistry, cellSize);
        enemySpawnGenerator = new EnemySpawnGenerator(mapGrid, transform, enemySpawner, levelEnemySpawnInfoList, cellSize);
        environmentGenerator = new EnvironmentGenerator(mapGrid, transform, environmentSpawnEntryList, itemRegistry, objectRegistry, cellSize);

        return true;
    }

    private async UniTask<bool> LoadGroundAsync(CancellationToken ct)
    {
        groundPrefab = await addressableLoader.LoadPrefabAsync(groundReference, ct);
        return groundPrefab != null;
    }

    private async UniTask<bool> LoadStructuresAsync(CancellationToken ct)
    {
        foreach (var entry in structureSpawnEntryList)
        {
            if (entry == null) continue;
            entry.prefab = await addressableLoader.LoadPrefabAsync(entry.prefabReference, ct);
            if (entry.prefab == null) return false;
        }
        return true;
    }

    private async UniTask<bool> LoadChestsAsync(CancellationToken ct)
    {
        if (levelChestSpawnInfoList == null) return true;

        Dictionary<string, UniTask<GameObject>> loadTasks = new();

        foreach (var levelInfo in levelChestSpawnInfoList)
        {
            if (levelInfo?.chestReferences == null) continue;

            foreach (var reference in levelInfo.chestReferences)
            {
                if (reference == null || string.IsNullOrEmpty(reference.AssetGUID)) continue;
                if (!loadTasks.ContainsKey(reference.AssetGUID)) loadTasks.Add(reference.AssetGUID, addressableLoader.LoadPrefabAsync(reference, ct));
            }
        }

        Dictionary<string, GameObject> loadedChests = new();

        foreach (var pair in loadTasks)
        {
            GameObject prefab = await pair.Value;
            if (prefab == null) return false;
            loadedChests.Add(pair.Key, prefab);
        }

        foreach (var levelInfo in levelChestSpawnInfoList)
        {
            if (levelInfo == null) continue;

            levelInfo.chestEntryList = new List<GameObject>();
            if (levelInfo.chestReferences == null) continue;

            foreach (var reference in levelInfo.chestReferences)
            {
                if (reference == null || string.IsNullOrEmpty(reference.AssetGUID)) continue;
                if (loadedChests.TryGetValue(reference.AssetGUID, out var prefab)) levelInfo.chestEntryList.Add(prefab);
            }
        }
        return true;
    }

    private async UniTask<bool> LoadItemSpotsAsync(CancellationToken ct)
    {
        foreach (var spot in itemSpotPrefabList)
        {
            if (spot == null) continue;
            spot.prefab = await addressableLoader.LoadPrefabAsync(spot.prefabReference, ct);
            if (spot.prefab == null) return false;
        }
        return true;
    }

    private async UniTask<bool> LoadEnemiesAsync(CancellationToken ct)
    {
        if (levelEnemySpawnInfoList == null) return true;

        Dictionary<string, UniTask<GameObject>> loadTasks = new();
        foreach (var levelInfo in levelEnemySpawnInfoList)
        {
            if (levelInfo?.spawnEntryList == null) continue;

            foreach (var entry in levelInfo.spawnEntryList)
            {
                if (entry == null || entry.spawnCount <= 0) continue;
                if (entry.prefabReference == null || string.IsNullOrEmpty(entry.prefabReference.AssetGUID)) continue;

                string key = entry.prefabReference.AssetGUID;
                if (!loadTasks.ContainsKey(key)) loadTasks.Add(key, addressableLoader.LoadPrefabAsync(entry.prefabReference, ct));
            }
        }

        Dictionary<string, GameObject> loadedEnemies = new();
        foreach (var pair in loadTasks)
        {
            GameObject prefab = await pair.Value;
            if (prefab == null) return false;
            loadedEnemies.Add(pair.Key, prefab);
        }

        foreach (var levelInfo in levelEnemySpawnInfoList)
        {
            if (levelInfo?.spawnEntryList == null) continue;

            foreach (var entry in levelInfo.spawnEntryList)
            {
                if (entry == null || entry.spawnCount <= 0) continue;
                if (entry.prefabReference == null || string.IsNullOrEmpty(entry.prefabReference.AssetGUID)) continue;
                if (loadedEnemies.TryGetValue(entry.prefabReference.AssetGUID, out var prefab)) entry.prefab = prefab;
            }
        }
        return true;
    }

    private async UniTask<bool> LoadEnvironmentsAsync(CancellationToken ct)
    {
        foreach (var entry in environmentSpawnEntryList)
        {
            if (entry == null) continue;
            entry.prefab = await addressableLoader.LoadPrefabAsync(entry.prefabReference, ct);
            if (entry.prefab == null) return false;
        }
        return true;
    }

    public async UniTask GenerateMapAsync(System.Action<float> onProgress, CancellationToken ct)
    {
        InitializeSeed();
        if (!await InitializeAsync(ct)) throw new System.InvalidOperationException("맵 리소스 로드 실패");

        onProgress?.Invoke(0.45f);
        await UniTask.NextFrame(ct);
        await groundGenerator.GenerateAsync(noiseOffsetX, noiseOffsetZ, ct);
        CreateCampFire();
        onProgress?.Invoke(0.60f);
        await UniTask.NextFrame(ct);
        await structureGenerator.GenerateAsync(structureRandom, ct);
        onProgress?.Invoke(0.70f);
        await UniTask.NextFrame(ct);
        await itemSpotGenerator.GenerateAsync(itemSpotRandom, ct);
        onProgress?.Invoke(0.75f);
        await UniTask.NextFrame(ct);
        await enemySpawnGenerator.GenerateAsync(enemyRandom, ct);
        onProgress?.Invoke(0.80f);
        await UniTask.NextFrame(ct);
        await environmentGenerator.GenerateAsync(environmentRandom, ct);

        onProgress?.Invoke(0.90f);
        await UniTask.NextFrame(ct);
        bool isReady = await navMeshGenerator.GenerateAsync(ct);

        ct.ThrowIfCancellationRequested();
        if (!isReady) throw new System.InvalidOperationException("NavMesh 생성 실패");
        onProgress?.Invoke(1f);
    }

    public void ActivateWorld()
    {
        if (player != null) player.gameObject.SetActive(true);
        enemySpawner.Initialize();
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
