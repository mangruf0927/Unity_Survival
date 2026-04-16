using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public PoolTypeEnums enemyType;
    public int enemyCount;
    public float respawnTime = 5f;
    public List<Transform> spawnPointList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;

    private void Start()
    {
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
        Transform spawnPoint = RandomSpwanPoint(info.spawnPointList);

        GameObject enemy = ObjectPool.Instance.GetFromPool(info.enemyType);
        enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        enemyStats.OnDead -= EnemyDead;
        enemyStats.OnDead += EnemyDead;
    }

    private void EnemyDead(EnemyStats enemyStats)
    {
        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            if (info.enemyType == enemyStats.EnemyType)
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
