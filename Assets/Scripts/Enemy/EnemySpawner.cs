using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public int groupId;
    public int enemyId;
    public int enemyCount;
    public float respawnTime = 5f;
    public List<Transform> spawnPointList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int requiredLevel;
    [SerializeField] private ItemDataBase itemDataBase;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EnemyHPBarController hpBarController;
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;

    private Dictionary<int, EnemySpawnInfo> groupInfoDictionary;
    public int RequiredLevel => requiredLevel;

    private readonly List<RespawnTimer> respawnTimerList = new();
    private readonly List<AliveEnemy> aliveEnemyList = new();

    private class RespawnTimer
    {
        public int groupId;
        public float remainingTime;
    }

    private class AliveEnemy
    {
        public int groupId;
        public EnemyController controller;
    }

    private void Awake()
    {
        InitializeGroups();
    }

    private void Start()
    {
        SpawnInitialEnemies();
    }

    private void Update()
    {
        UpdateRespawnTimer();
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

                groupData.aliveEnemyDataList.Add(enemy.controller.CreateSaveData());
            }

            foreach (RespawnTimer timer in respawnTimerList)
            {
                if (timer.groupId != info.groupId) continue;

                groupData.respawnSaveDataList.Add(new EnemyRespawnSaveData
                {
                    remainingTime = Mathf.Max(0f, timer.remainingTime)
                });
            }
            groupDataList.Add(groupData);
        }
        return groupDataList;
    }

    public void LoadSaveData(List<EnemyGroupSaveData> dataList)
    {
        InitializeGroups();
        ClearList();

        if (dataList == null)
        {
            SpawnInitialEnemies();
            return;
        }

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null) continue;

            EnemyGroupSaveData groupData = dataList.Find(x => x.groupId == info.groupId);

            if (groupData == null)
            {
                for (int i = 0; i < info.enemyCount; i++)
                {
                    SpawnEnemy(info.groupId);
                }
                continue;
            }

            foreach (EnemySaveData enemyData in groupData.aliveEnemyDataList)
            {
                SpawnSavedEnemy(info.groupId, enemyData);
            }

            foreach (EnemyRespawnSaveData respawnData in groupData.respawnSaveDataList)
            {
                respawnTimerList.Add(new RespawnTimer
                {
                    groupId = info.groupId,
                    remainingTime = Mathf.Max(0f, respawnData.remainingTime)
                });
            }

            int currentCount = groupData.aliveEnemyDataList.Count + groupData.respawnSaveDataList.Count;
            int missingCount = info.enemyCount - currentCount;

            for (int i = 0; i < missingCount; i++)
                SpawnEnemy(info.groupId);
        }
    }

    private void SpawnSavedEnemy(int groupId, EnemySaveData saveData)
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

        RegisterEnemy(groupId, enemyController);
    }

    private void ClearList()
    {
        foreach (AliveEnemy enemy in aliveEnemyList)
        {
            if (enemy.controller == null) continue;

            ObjectPool.Instance.ReturnToPool(enemy.controller.gameObject, enemy.controller.EnemyType);
        }
        aliveEnemyList.Clear();
        respawnTimerList.Clear();
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

    private void SpawnInitialEnemies()
    {
        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null) continue;

            for (int i = 0; i < info.enemyCount; i++)
                SpawnEnemy(info.groupId);
        }
    }

    private void UpdateRespawnTimer()
    {
        for (int i = respawnTimerList.Count - 1; i >= 0; i--)
        {
            RespawnTimer timer = respawnTimerList[i];

            timer.remainingTime -= Time.deltaTime;

            if (timer.remainingTime > 0) continue;

            respawnTimerList.RemoveAt(i);
            SpawnEnemy(timer.groupId);
        }
    }

    private void SpawnEnemy(int groupId)
    {
        if (!groupInfoDictionary.TryGetValue(groupId, out EnemySpawnInfo info)) return;

        EnemyData data = DataManager.Instance.EnemyTable.Get(info.enemyId);
        Transform spawnPoint = RandomSpawnPoint(info.spawnPointList);
        if (spawnPoint == null) return;

        GameObject enemy = ObjectPool.Instance.GetFromPool(data.EnemyType);
        if (enemy == null) return;

        enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

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

        RegisterEnemy(groupId, enemyController);
    }

    private void RegisterEnemy(int groupId, EnemyController controller)
    {
        aliveEnemyList.Add(new AliveEnemy
        {
            groupId = groupId,
            controller = controller
        });
    }

    private void EnemyDead(EnemyStats enemyStats)
    {
        AliveEnemy aliveEnemy = aliveEnemyList.Find(x => x.controller != null
                                && x.controller.GetComponent<EnemyStats>() == enemyStats);

        if (aliveEnemy == null) return;

        aliveEnemyList.Remove(aliveEnemy);

        if (groupInfoDictionary.TryGetValue(aliveEnemy.groupId, out EnemySpawnInfo info))
        {
            respawnTimerList.Add(new RespawnTimer
            {
                groupId = aliveEnemy.groupId,
                remainingTime = info.respawnTime
            });
        }
    }

    private Transform RandomSpawnPoint(List<Transform> spawnList)
    {
        if (spawnList == null || spawnList.Count == 0) return null;

        int idx = UnityEngine.Random.Range(0, spawnList.Count);
        return spawnList[idx];
    }

    private bool GetEnemyType(int enemyId, out PoolTypeEnums ememyType)
    {
        ememyType = default;

        if (enemyId <= 0) return false;
        if (!DataManager.Instance.EnemyTable.TryGet(enemyId, out EnemyData enemyData)) return false;

        ememyType = enemyData.EnemyType;
        return true;
    }
}
