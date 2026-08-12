using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour
{
    [SerializeField] private ObjectDataBase objectDataBase;
    [SerializeField] private List<Transform> objectRootList = new();

    private readonly Dictionary<long, WorldObject> worldObjectDictionary = new();
    private readonly List<WorldObject> generatedObjectList = new();
    private readonly List<WorldObject> runtimeObjectList = new();

    private const long StartRuntimeId = 1;
    private const long StartGeneratedId = 1000000000;

    private long nextRuntimeId = StartRuntimeId;
    private long nextGeneratedId = StartGeneratedId;

    private void Awake()
    {
        RegisterSceneObjects();
    }

    private void RegisterSceneObjects()
    {
        foreach (Transform root in objectRootList)
        {
            if (root == null) continue;

            RegisterGeneratedObjects(root.gameObject);
        }
    }

    public void RegisterGeneratedObjects(GameObject root)
    {
        if (root == null) return;

        WorldObject[] objects = root.GetComponentsInChildren<WorldObject>(true);

        foreach (WorldObject obj in objects)
        {
            RegisterGenerated(obj);
        }
    }

    public void RegisterGenerated(WorldObject obj)
    {
        if (obj == null) return;

        if (obj.ObjectType != ObjectType.GENERATED)
        {
            Debug.LogWarning($"{obj.name}: ObjectType is not GENERATED.", obj);
        }

        if (obj.InstanceId < StartGeneratedId)
        {
            obj.SetInstanceId(CreateGeneratedId());
        }

        if (!generatedObjectList.Contains(obj))
        {
            generatedObjectList.Add(obj);
        }

        AddToDictionary(obj);
    }

    public void RegisterRuntime(WorldObject obj)
    {
        if (obj == null) return;

        if (obj.ObjectType != ObjectType.RUNTIME)
        {
            Debug.LogWarning($"{obj.name}: ObjectType is not RUNTIME.", obj);
        }

        if (obj.InstanceId <= 0 || obj.InstanceId >= StartGeneratedId)
        {
            obj.SetInstanceId(CreateRuntimeId());
        }

        if (!runtimeObjectList.Contains(obj))
        {
            runtimeObjectList.Add(obj);
        }

        AddToDictionary(obj);
        UpdateRuntimeId(obj.InstanceId);
    }

    public void Unregister(WorldObject obj)
    {
        if (obj == null || obj.InstanceId <= 0) return;

        worldObjectDictionary.Remove(obj.InstanceId);
        generatedObjectList.Remove(obj);
        runtimeObjectList.Remove(obj);
    }

    private void AddToDictionary(WorldObject obj)
    {
        if (obj == null || obj.InstanceId <= 0) return;

        worldObjectDictionary[obj.InstanceId] = obj;
    }

    private long CreateRuntimeId()
    {
        while (worldObjectDictionary.ContainsKey(nextRuntimeId))
        {
            nextRuntimeId++;
        }

        return nextRuntimeId++;
    }

    private long CreateGeneratedId()
    {
        while (worldObjectDictionary.ContainsKey(nextGeneratedId))
        {
            nextGeneratedId++;
        }

        return nextGeneratedId++;
    }

    private void UpdateRuntimeId(long instanceId)
    {
        if (instanceId >= nextRuntimeId && instanceId < StartGeneratedId)
        {
            nextRuntimeId = instanceId + 1;
        }
    }

    // Save/Load
    public WorldSaveData CreateSaveData()
    {
        return new WorldSaveData
        {
            nextInstanceId = nextRuntimeId,
            objectSaveDataList = CreateObjectSaveData()
        };
    }

    public void LoadSaveData(WorldSaveData data)
    {
        if (data == null) return;

        nextRuntimeId = Math.Max(data.nextInstanceId, StartRuntimeId);

        if (data.objectSaveDataList != null)
        {
            foreach (ObjectSaveData objectData in data.objectSaveDataList)
            {
                if (objectData == null || objectData.objectType != ObjectType.RUNTIME) continue;

                UpdateRuntimeId(objectData.instanceId);
            }
        }
        ClearRuntimeObjects();
        LoadObjectSaveData(data.objectSaveDataList);
    }

    private List<ObjectSaveData> CreateObjectSaveData()
    {
        List<ObjectSaveData> dataList = new();

        foreach (WorldObject obj in worldObjectDictionary.Values)
        {
            if (obj == null) continue;

            dataList.Add(obj.CreateSaveData());
        }

        return dataList;
    }

    private void LoadObjectSaveData(List<ObjectSaveData> dataList)
    {
        if (dataList == null) return;

        foreach (ObjectSaveData data in dataList)
        {
            if (data == null) continue;

            if (worldObjectDictionary.TryGetValue(data.instanceId, out WorldObject existingObject))
            {
                existingObject.LoadSaveData(data);
                continue;
            }

            if (data.objectType == ObjectType.RUNTIME)
            {
                SpawnRuntimeObject(data);
            }
        }
    }

    private void ClearRuntimeObjects()
    {
        foreach (WorldObject obj in runtimeObjectList)
        {
            if (obj == null) continue;

            worldObjectDictionary.Remove(obj.InstanceId);
            Destroy(obj.gameObject);
        }
        runtimeObjectList.Clear();
    }

    private void SpawnRuntimeObject(ObjectSaveData data)
    {
        if (objectDataBase == null) return;

        WorldObject prefab = objectDataBase.GetPrefab(data.itemId);

        if (prefab == null)
        {
            Debug.LogError($"WorldObject prefab not found. ItemId: {data.itemId}");
            return;
        }

        Vector3 position = new(data.positionX, data.positionY, data.positionZ);
        Quaternion rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);

        WorldObject obj = Instantiate(prefab, position, rotation);
        obj.SetInstanceId(data.instanceId);
        obj.LoadSaveData(data);

        RegisterRuntime(obj);
    }
}
