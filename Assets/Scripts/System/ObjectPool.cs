using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{    
    [SerializeField] private List<PoolData> poolDataList;

    public static ObjectPool Instance { get; private set; }
    
    private readonly List<Stack<GameObject>> poolList = new();
    private readonly Dictionary<PoolTypeEnums, Transform> parentDictionary = new();

    private void Awake() 
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (PoolTypeEnums type in Enum.GetValues(typeof(PoolTypeEnums)))
        {
            poolList.Add(new Stack<GameObject>());

            GameObject parent = new(type.ToString());
            parent.transform.SetParent(transform);
            parentDictionary.Add(type, parent.transform);
        }    

        foreach (var data in poolDataList)
        {
            InitializePool(data.size, data.prefab, data.poolType, parentDictionary[data.poolType]);
        }
    }

    private void InitializePool(int poolSize, GameObject prefab, PoolTypeEnums poolType, Transform parent = null)
    {
        var pool = poolList[(int)poolType];

        for (int i = 0; i < poolSize; i++)
        {
            pool.Push(CreatePool(prefab, parent));
        }
    }

    public GameObject GetFromPool(PoolTypeEnums poolType)
    {
        var pool = poolList[(int)poolType];

        GameObject obj;
        if(pool.Count > 0) obj = pool.Pop();
        else
        {
            var data = poolDataList.Find(x => x.poolType == poolType);
            if(data == null) return null;
            obj = CreatePool(data.prefab);
        }
        obj.transform.SetParent(parentDictionary[poolType], false);
        obj.SetActive(true);
        return obj;
    }

    private GameObject CreatePool(GameObject prefab, Transform parent = null)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public void ReturnToPool(GameObject obj, PoolTypeEnums poolType)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parentDictionary[poolType]);
        poolList[(int)poolType].Push(obj);
    }
}
