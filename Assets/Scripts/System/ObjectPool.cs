using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly Dictionary<PoolTypeEnums, Stack<GameObject>> poolDictionary = new();
    private readonly Dictionary<PoolTypeEnums, PoolData> dataDictionary = new();
    private readonly Dictionary<PoolTypeEnums, Transform> parentDictionary = new();

    public static ObjectPool Instance { get; } = new();

    private Transform inactiveRoot;

    public void Register(PoolData data, Transform parent)
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError($"{data?.poolType} pool prefab is missing.");
            return;
        }

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
        GameObject obj = TakeFromPool(poolType);
        if (obj == null) return null;

        obj.SetActive(true);
        return obj;
    }

    public GameObject GetFromPool(PoolTypeEnums poolType, Vector3 position, Quaternion rotation)
    {
        GameObject obj = TakeFromPool(poolType);
        if (obj == null) return null;

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    private GameObject TakeFromPool(PoolTypeEnums poolType)
    {
        if (!poolDictionary.TryGetValue(poolType, out Stack<GameObject> pool)) return null;
        if (!dataDictionary.TryGetValue(poolType, out PoolData data)) return null;

        GameObject obj;
        if (pool.Count > 0) obj = pool.Pop();
        else obj = CreatePool(data.prefab, parentDictionary[poolType]);

        obj.transform.SetParent(parentDictionary[poolType], false);
        return obj;
    }

    private GameObject CreatePool(GameObject prefab, Transform parent = null)
    {
        GameObject obj = Object.Instantiate(prefab, GetInactiveRoot());
        obj.SetActive(false);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private Transform GetInactiveRoot()
    {
        if (inactiveRoot != null) return inactiveRoot;

        GameObject root = new("InactiveRoot");
        root.SetActive(false);
        Object.DontDestroyOnLoad(root);

        inactiveRoot = root.transform;
        return inactiveRoot;
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
