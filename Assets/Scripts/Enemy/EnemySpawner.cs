using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemySpawnInfo
{
    public int groupId;
    public int enemyId;
    public float spawnRadius = 3f;
    public List<Transform> spawnPointList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int requiredLevel;
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private ItemDataBase itemDataBase;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EnemyHPBarController hpBarController;
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;

    private readonly List<AliveEnemy> aliveEnemyList = new();
    private Dictionary<int, EnemySpawnInfo> groupInfoDictionary;

    public int RequiredLevel => requiredLevel;
    private int lastCycle = 0;

    private class AliveEnemy
    {
        public int groupId;
        public int spawnIndex;
        public EnemyController controller;
    }

    private void Awake()
    {
        InitializeGroups();
    }

    private void OnEnable()
    {
        if (timeSystem != null) timeSystem.OnPhaseChanged += PhaseChanged;
    }

    private void OnDisable()
    {
        if (timeSystem != null) timeSystem.OnPhaseChanged -= PhaseChanged;
    }

    public List<EnemyGroupSaveData> CreateSaveData()
    {
        List<EnemyGroupSaveData> groupDataList = new();

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null) continue;

            EnemyGroupSaveData groupData = new()
            {
                groupId = info.groupId,
                aliveEnemyDataList = new List<EnemySaveData>(),
                respawnSaveDataList = new List<EnemyRespawnSaveData>()
            };

            foreach (AliveEnemy enemy in aliveEnemyList)
            {
                if (enemy.groupId != info.groupId) continue;
                if (enemy.controller == null || enemy.controller.CurrentHp <= 0) continue;

                EnemySaveData enemySaveData = enemy.controller.CreateSaveData();
                enemySaveData.spawnIndex = enemy.spawnIndex;
                groupData.aliveEnemyDataList.Add(enemySaveData);
            }

            groupDataList.Add(groupData);
        }
        return groupDataList;
    }

    public void LoadSaveData(List<EnemyGroupSaveData> dataList)
    {
        InitializeGroups();
        ClearList();

        if (dataList == null) return;

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null) continue;

            EnemyGroupSaveData groupData = dataList.Find(x => x.groupId == info.groupId);

            if (groupData == null) continue;

            foreach (EnemySaveData data in groupData.aliveEnemyDataList)
            {
                int spawnIndex = GetSpawnIndex(info, data.spawnIndex);

                if (spawnIndex < 0) continue;

                SpawnSavedEnemy(info.groupId, spawnIndex, data);
            }
        }
    }

    private void SpawnSavedEnemy(int groupId, int spawnIndex, EnemySaveData saveData)
    {
        EnemyData data = DataManager.Instance.EnemyTable.Get(saveData.enemyId);
        GameObject enemy = ObjectPool.Instance.GetFromPool(data.EnemyType);
        if (enemy == null) return;

        if (enemy.TryGetComponent(out EnemyDropper enemyDropper))
        {
            enemyDropper.SetUp(itemDataBase, itemRegistry);
        }

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyStats == null || enemyController == null)
        {
            ObjectPool.Instance.ReturnToPool(enemy, data.EnemyType);
            return;
        }

        enemyStats.SetHPBarController(hpBarController);
        enemyController.LoadSaveData(saveData, data);

        enemyStats.OnDead -= EnemyDead;
        enemyStats.OnDead += EnemyDead;

        RegisterEnemy(groupId, spawnIndex, enemyController);
    }

    private void ClearList()
    {
        foreach (AliveEnemy enemy in aliveEnemyList)
        {
            if (enemy.controller == null) continue;

            ObjectPool.Instance.ReturnToPool(enemy.controller.gameObject, enemy.controller.EnemyType);
        }
        aliveEnemyList.Clear();
    }

    private void InitializeGroups()
    {
        groupInfoDictionary = new Dictionary<int, EnemySpawnInfo>();

        if (spawnInfoList == null) return;

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null) continue;
            if (groupInfoDictionary.ContainsKey(info.groupId)) continue;

            groupInfoDictionary.Add(info.groupId, info);
        }
    }

    private void PhaseChanged(Phase phase, int day)
    {
        if (phase != Phase.DAY) return;

        int currentCycle = timeSystem.CycleCount;
        if (lastCycle == currentCycle) return;
        lastCycle = currentCycle;

        SpawnEnemies();
    }

    private bool IsEnemyAlive(int groupId, int spawnIndex)
    {
        return aliveEnemyList.Exists(x => x.groupId == groupId &&
                                        x.spawnIndex == spawnIndex &&
                                        x.controller != null && x.controller.CurrentHp > 0);
    }

    private void SpawnEnemies()
    {
        if (spawnInfoList == null) return;

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null || info.spawnPointList == null) continue;

            for (int i = 0; i < info.spawnPointList.Count; i++)
            {
                if (IsEnemyAlive(info.groupId, i)) continue;
                SpawnEnemy(info.groupId, i);
            }
        }
    }

    private void SpawnEnemy(int groupId, int spawnIndex)
    {
        if (!groupInfoDictionary.TryGetValue(groupId, out EnemySpawnInfo info)) return;
        if (info.spawnPointList == null || spawnIndex < 0 || spawnIndex >= info.spawnPointList.Count) return;

        Transform spawnPoint = info.spawnPointList[spawnIndex];
        if (spawnPoint == null) return;

        EnemyData data = DataManager.Instance.EnemyTable.Get(info.enemyId);
        GameObject enemy = ObjectPool.Instance.GetFromPool(data.EnemyType);
        if (enemy == null) return;

        Vector3 spawnPosition = GetSpawnPosition(spawnPoint, info.spawnRadius);
        enemy.transform.SetPositionAndRotation(spawnPosition, spawnPoint.rotation);

        if (enemy.TryGetComponent(out EnemyDropper enemyDropper))
        {
            enemyDropper.SetUp(itemDataBase, itemRegistry);
        }

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyStats == null || enemyController == null)
        {
            ObjectPool.Instance.ReturnToPool(enemy, data.EnemyType);
            return;
        }

        enemyStats.SetHPBarController(hpBarController);
        enemyStats.SetUp(data);

        enemyStats.OnDead -= EnemyDead;
        enemyStats.OnDead += EnemyDead;

        RegisterEnemy(groupId, spawnIndex, enemyController);
    }

    private void RegisterEnemy(int groupId, int spawnIndex, EnemyController controller)
    {
        aliveEnemyList.Add(new AliveEnemy
        {
            groupId = groupId,
            spawnIndex = spawnIndex,
            controller = controller
        });
    }

    private void EnemyDead(EnemyStats enemyStats)
    {
        AliveEnemy aliveEnemy = aliveEnemyList.Find(x => x.controller != null
                                && x.controller.GetComponent<EnemyStats>() == enemyStats);

        if (aliveEnemy == null) return;

        aliveEnemyList.Remove(aliveEnemy);
    }

    private int GetSpawnIndex(EnemySpawnInfo info, int index)
    {
        bool isValidIndex = index >= 0 && index < info.spawnPointList.Count;
        if (isValidIndex && !IsEnemyAlive(info.groupId, index)) return index;

        for (int i = 0; i < info.spawnPointList.Count; i++)
        {
            if (!IsEnemyAlive(info.groupId, i)) return i;
        }

        return -1;
    }

    private Vector3 GetSpawnPosition(Transform spawnPoint, float radius)
    {
        if (radius <= 0f) return spawnPoint.position;

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 randomPos = spawnPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        if (NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit fallbackHit, 2f, NavMesh.AllAreas))
        {
            return fallbackHit.position;
        }

        return spawnPoint.position;
    }
}
