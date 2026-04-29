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
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;

    private IEnumerator Start()
    {
        if (!DataManager.Instance.IsLoaded)
        {
            yield return DataManager.Instance.LoadAll();
        }

        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            for (int i = 0; i < info.enemyCount; i++)
            {
                SpawnEnemy(info);
            }
        }
    }

    private void SpawnEnemy(EnemySpawnInfo info)
    {
        EnemyDataTable data = DataManager.Instance.EnemyTable.Get(info.enemyId);

        Transform spawnPoint = RandomSpwanPoint(info.spawnPointList);

        GameObject enemy = ObjectPool.Instance.GetFromPool(data.EnemyType);
        enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        enemyStats.SetUp(data);

        enemyStats.OnDead -= EnemyDead;
        enemyStats.OnDead += EnemyDead;
    }

    private void EnemyDead(EnemyStats enemyStats)
    {
        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            EnemyDataTable data = DataManager.Instance.EnemyTable.Get(info.enemyId);
            if (data == null) continue;

            if (data.EnemyType == enemyStats.EnemyType)
            {
                StartCoroutine(Respawn(info));
                break;
            }
        }
    }

    private IEnumerator Respawn(EnemySpawnInfo info)
    {
        yield return new WaitForSeconds(info.respawnTime);
        SpawnEnemy(info);
    }

    private Transform RandomSpwanPoint(List<Transform> spawnList)
    {
        int idx = UnityEngine.Random.Range(0, spawnList.Count);
        return spawnList[idx];
    }
}
