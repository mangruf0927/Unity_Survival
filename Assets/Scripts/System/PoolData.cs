using System;
using UnityEngine;

[Serializable]
public class PoolData
{
    public PoolTypeEnums poolType;
    public GameObject prefab;
    public Transform parent;
    public int size;
}
