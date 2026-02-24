using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolTypeEnums { BULLET }

public class ObjectPool : MonoBehaviour
{    
    private readonly List<Stack<GameObject>> poolList = new();

    private void Awake() 
    {
        foreach (PoolTypeEnums type in Enum.GetValues(typeof(PoolTypeEnums)))
        {
            poolList.Add(new Stack<GameObject>());
        }    
    }

    public void InitializePool(int poolSize, GameObject prefab, PoolTypeEnums poolType, Transform parent = null)
    {
        var pool = poolList[(int)poolType];

        for(int i = 0; i < poolSize; i++)
        {
            pool.Push(CreatePool(prefab, parent));
        }
    }

    public GameObject GetFromPool(GameObject prefab, PoolTypeEnums poolType, Transform parent = null)
    {
        var pool = poolList[(int)poolType];

        GameObject obj = pool.Count > 0 ? pool.Pop() : CreatePool(prefab, parent);
        obj.transform.SetParent(parent, false);
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
        poolList[(int)poolType].Push(obj);
    }
}
