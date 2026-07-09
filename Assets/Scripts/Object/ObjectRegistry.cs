using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour
{
    [SerializeField] private List<WorldObject> worldObjectList = new();
    [SerializeField] private ObjectDataBase objectDataBase;

    private readonly Dictionary<long, WorldObject> worldObjectMap = new();
    private readonly List<WorldObject> runtimeObjectList = new();

    private const long StartId = 1;
    private long nextInstanceId = StartId;

    private void Awake()
    {
        RegisterObjects();
    }

    public long CreateInstanceId()
    {
        return nextInstanceId++;
    }

    public void Register(WorldObject obj)
    {
        if (obj == null || obj.InstanceId <= 0) return;

        worldObjectMap[obj.InstanceId] = obj;

        if (!worldObjectList.Contains(obj) && IsDynamicObject(obj.ObjectType) && !runtimeObjectList.Contains(obj))
        {
            runtimeObjectList.Add(obj);
        }
    }

    public void Unregister(WorldObject obj)
    {
        if (obj == null || obj.InstanceId <= 0) return;

        worldObjectMap.Remove(obj.InstanceId);
        runtimeObjectList.Remove(obj);
    }

    public WorldSaveData CreateSaveData()
    {
        RegisterObjects();

        return new WorldSaveData
        {
            nextInstanceId = nextInstanceId,
            objectSaveDataList = CreateObjectSaveData()
        };
    }

    private List<ObjectSaveData> CreateObjectSaveData()
    {
        List<ObjectSaveData> dataList = new();

        foreach (WorldObject obj in worldObjectMap.Values)
        {
            if (obj == null) continue;
            dataList.Add(obj.CreateSaveData());
        }

        return dataList;
    }

    public void LoadSaveData(WorldSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("World object save data is missing. Save again after changing tree/chest state.");
            return;
        }

        nextInstanceId = Math.Max(data.nextInstanceId, StartId);
        UpdateNextInstanceId(data.objectSaveDataList);

        ClearRuntimeObjects();
        RegisterObjects();
        LoadObjectSaveData(data.objectSaveDataList);
    }

    private void LoadObjectSaveData(List<ObjectSaveData> dataList)
    {
        if (dataList == null) return;

        foreach (ObjectSaveData data in dataList)
        {
            if (worldObjectMap.TryGetValue(data.instanceId, out WorldObject sceneObj))
            {
                sceneObj.LoadSaveData(data);
                continue;
            }

            if (IsDynamicObject(data.objectType))
            {
                SpawnDynamicObject(data);
            }
        }
    }

    private void RegisterObjects()
    {
        worldObjectMap.Clear();

        foreach (WorldObject obj in worldObjectList)
        {
            if (obj == null || obj.InstanceId <= 0) continue;
            Register(obj);
        }

        long fallbackId = StartId;
        foreach (WorldObject obj in worldObjectList)
        {
            if (obj == null || obj.InstanceId > 0) continue;

            while (worldObjectMap.ContainsKey(fallbackId))
            {
                fallbackId++;
            }

            obj.SetInstanceId(fallbackId);
            Register(obj);
        }
    }

    private void ClearRuntimeObjects()
    {
        foreach (WorldObject obj in runtimeObjectList)
        {
            if (obj == null) continue;
            Destroy(obj.gameObject);
        }

        runtimeObjectList.Clear();
    }

    private bool IsDynamicObject(ObjectType objectType)
    {
        return objectType == ObjectType.PLACEABLE;
    }

    private void UpdateNextInstanceId(List<ObjectSaveData> dataList)
    {
        if (dataList == null) return;

        foreach (ObjectSaveData data in dataList)
        {
            if (data.instanceId >= nextInstanceId)
            {
                nextInstanceId = data.instanceId + 1;
            }
        }
    }

    private void SpawnDynamicObject(ObjectSaveData data)
    {
        if (objectDataBase == null) return;

        WorldObject prefab = objectDataBase.GetPrefab(data.itemId);
        if (prefab == null) return;

        Vector3 position = new(data.positionX, data.positionY, data.positionZ);
        Quaternion rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);

        WorldObject obj = Instantiate(prefab, position, rotation);
        obj.SetInstanceId(data.instanceId);
        obj.LoadSaveData(data);

        Register(obj);
    }
}
