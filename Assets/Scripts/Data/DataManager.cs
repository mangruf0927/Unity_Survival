using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

public class DataManager
{
    public static DataManager Instance { get; } = new();

    public DataTable<EnemyData> EnemyTable { get; } = new();
    public DataTable<PlayerData> PlayerTable { get; } = new();
    public DataTable<SackData> SackTable { get; } = new();
    public DataTable<MeleeWeaponData> MeleeTable { get; } = new();
    public DataTable<RangedWeaponData> RangedTable { get; } = new();
    public DataTable<ItemData> ItemTable { get; } = new();

    public bool IsLoaded { get; private set; }

    private readonly DataTableValidator validator = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        Instance.LoadAll();
    }

    public void LoadAll()
    {
        if (IsLoaded) return;

        TextAsset[] textAssets = JsonDataLoader.LoadAll("JSON");
        foreach (TextAsset textAsset in textAssets)
        {
            LoadTable(textAsset);
        }

        if (!validator.Validate())
        {
            Debug.LogError("DataTable validation failed");
        }

        IsLoaded = true;
        Debug.Log("DataManager Load Complete");
    }

    private void LoadTable(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("JSON file is null");
            return;
        }

        string json = textAsset.text;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError($"JSON is empty: {textAsset.name}");
            return;
        }

        // tableName, records 꺼내기
        JObject root = JObject.Parse(json);
        string tableName = root["tableName"]?.ToString();
        JToken recordsToken = root["records"];

        if (string.IsNullOrEmpty(tableName) || recordsToken == null)
        {
            Debug.LogError($"tableName or records is missing: {textAsset.name}");
            return;
        }

        // DataManager에서 tableName과 같은 이름의 프로퍼티 찾기
        PropertyInfo tableProperty = GetType().GetProperty(tableName);
        if (tableProperty == null)
        {
            Debug.LogError($"Table property not found : {tableName}");
            return;
        }

        // DataTable<T> 타입인지 확인
        Type tableType = tableProperty.PropertyType;
        if (!tableType.IsGenericType || tableType.GetGenericTypeDefinition() != typeof(DataTable<>))
        {
            Debug.LogError($"{tableName} is not DataTable<T>");
            return;
        }

        // List<T> 타입 만들기
        Type dataType = tableType.GetGenericArguments()[0];
        Type listType = typeof(List<>).MakeGenericType(dataType);

        object records = recordsToken.ToObject(listType);
        if (records == null)
        {
            Debug.LogError($"JSON Deserialize failed: {textAsset.name}");
            return;
        }

        // Load 메서드 호출
        object table = tableProperty.GetValue(this);
        MethodInfo method = tableType.GetMethod("Load");

        if (method == null)
        {
            Debug.LogError($"Load method not found: {tableName}");
            return;
        }
        method.Invoke(table, new object[] { records });
    }
}
