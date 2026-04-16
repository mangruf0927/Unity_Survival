using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public PoolTypeEnums enemyType;
    public int enemyCount;
    public List<Transform> spawnPointList;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnInfo> spawnInfoList;

    private void Start()
    {
        SpwanEnemies();
    }

    private void SpwanEnemies()
    {
        foreach (EnemySpawnInfo info in spawnInfoList)
        {
            for (int i = 0; i < info.enemyCount; i++)
            {
                Transform spawnPoint = RandomSpwanPoint(info.spawnPointList);

                GameObject enemy = ObjectPool.Instance.GetFromPool(info.enemyType);
                enemy.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    private Transform RandomSpwanPoint(List<Transform> spawnList)
    {
        int idx = UnityEngine.Random.Range(0, spawnList.Count);
        return spawnList[idx];
    }
}
