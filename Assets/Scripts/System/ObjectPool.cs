using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly Dictionary<PoolTypeEnums, Stack<GameObject>> poolDictionary = new();
    private readonly Dictionary<PoolTypeEnums, PoolData> dataDictionary = new();
    private readonly Dictionary<PoolTypeEnums, Transform> parentDictionary = new();

    public static ObjectPool Instance { get; } = new();

    public void Register(PoolData data, Transform parent)
    {
        if (dataDictionary.ContainsKey(data.poolType)) return;

        dataDictionary.Add(data.poolType, data);
        poolDictionary.Add(data.poolType, new Stack<GameObject>());

        Transform poolParent = data.parent != null ? data.parent : parent;
        parentDictionary.Add(data.poolType, poolParent);

        InitializePool(data.size, data.prefab, data.poolType, poolParent);
    }

    private void InitializePool(int poolSize, GameObject prefab, PoolTypeEnums poolType, Transform parent = null)
    {
        Stack<GameObject> pool = poolDictionary[poolType];

        for (int i = 0; i < poolSize; i++)
        {
            pool.Push(CreatePool(prefab, parent));
        }
    }

    public GameObject GetFromPool(PoolTypeEnums poolType)
    {
        if (!poolDictionary.TryGetValue(poolType, out Stack<GameObject> pool)) return null;
        if (!dataDictionary.TryGetValue(poolType, out PoolData data)) return null;

        GameObject obj;
        if (pool.Count > 0) obj = pool.Pop();
        else obj = CreatePool(data.prefab, parentDictionary[poolType]);

        obj.transform.SetParent(parentDictionary[poolType], false);
        obj.SetActive(true);
        return obj;
    }

    private GameObject CreatePool(GameObject prefab, Transform parent = null)
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public void ReturnToPool(GameObject obj, PoolTypeEnums poolType)
    {
        if (!poolDictionary.ContainsKey(poolType) ||
            !dataDictionary.ContainsKey(poolType) ||
            !parentDictionary.ContainsKey(poolType)) return;

        obj.SetActive(false);
        obj.transform.SetParent(parentDictionary[poolType], false);
        poolDictionary[poolType].Push(obj);
    }
}
