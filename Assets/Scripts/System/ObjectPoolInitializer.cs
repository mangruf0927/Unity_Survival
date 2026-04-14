using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolInitializer : MonoBehaviour
{
    [SerializeField] private List<PoolData> poolDataList;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        foreach (var data in poolDataList)
        {
            ObjectPool.Instance.Register(data, transform);
        }
    }
}