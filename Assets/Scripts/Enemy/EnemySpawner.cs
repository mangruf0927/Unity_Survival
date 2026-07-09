using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public int enemyId;
    public int enemyCount;
    public float respawnTime = 5f;
    public List<Transform> spawnPointList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int requiredLevel;
    [SerializeField] private ItemDataBase itemDataBase;
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private EnemyHPBarController hpBarController;

    private Dictionary<PoolTypeEnums, EnemySpawnInfo> typeToInfoDictionary;
    public int RequiredLevel => requiredLevel;

    private void Start()
    {
        typeToInfoDictionary = new Dictionary<PoolTypeEnums, EnemySpawnInfo>();

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info == null)
            {
                Debug.LogError("SpawnInfo가 비어있습니다.");
                continue;
            }

            if (info.spawnPointList == null || info.spawnPointList.Count == 0)
            {
                Debug.LogError($"{info.enemyId} 스폰 포인트가 비어있습니다.");
                continue;
            }

            if (!GetEnemyType(info.enemyId, out PoolTypeEnums enemyType)) continue;

            if (typeToInfoDictionary.ContainsKey(enemyType))
            {
                Debug.LogError("중복된 EnemyType 입니다.");
                continue;
            }

            typeToInfoDictionary.Add(enemyType, info);

            for (int i = 0; i < info.enemyCount; i++)
            {
                SpawnEnemy(info);
            }
        }
    }

    private bool GetEnemyType(int enemyId, out PoolTypeEnums ememyType)
    {
        ememyType = default;

        if (enemyId <= 0) return false;
        if (!DataManager.Instance.EnemyTable.TryGet(enemyId, out EnemyData enemyData)) return false;

        ememyType = enemyData.EnemyType;
        return true;
    }

    private void SpawnEnemy(EnemySpawnInfo info)
    {
        EnemyData data = DataManager.Instance.EnemyTable.Get(info.enemyId);

        Transform spawnPoint = RandomSpawnPoint(info.spawnPointList);
        if (spawnPoint == null)
        {
            Debug.LogError($"{info.enemyId} 스폰 포인트가 비어있습니다.");
            return;
        }

        GameObject enemy = ObjectPool.Instance.GetFromPool(data.EnemyType);
        if (enemy == null)
        {
            Debug.LogError($"{data.EnemyType} 풀이 등록되어 있지 않습니다.");
            return;
        }

        enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (enemy.TryGetComponent(out EnemyDropper enemyDropper))
        {
            enemyDropper.SetUp(itemDataBase, itemRegistry);
        }

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError($"{enemy.name}에 EnemyStats가 없습니다.");
            ObjectPool.Instance.ReturnToPool(enemy, data.EnemyType);
            return;
        }

        enemyStats.SetHPBarController(hpBarController);
        enemyStats.SetUp(data);

        enemyStats.OnDead -= EnemyDead;
        enemyStats.OnDead += EnemyDead;
    }

    private void EnemyDead(EnemyStats enemyStats)
    {
        if (typeToInfoDictionary.TryGetValue(enemyStats.EnemyType, out EnemySpawnInfo info))
        {
            StartCoroutine(Respawn(info));
        }
    }

    private IEnumerator Respawn(EnemySpawnInfo info)
    {
        yield return new WaitForSeconds(info.respawnTime);
        SpawnEnemy(info);
    }

    private Transform RandomSpawnPoint(List<Transform> spawnList)
    {
        int idx = UnityEngine.Random.Range(0, spawnList.Count);
        return spawnList[idx];
    }
}
