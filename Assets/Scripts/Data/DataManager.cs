using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

public class DataManager
{
    public static DataManager Instance { get; } = new();

    public DataTable<EnemyDataTable> EnemyTable { get; } = new();
    public DataTable<PlayerDataTable> PlayerTable { get; } = new();
    public DataTable<SackDataTable> SackTable { get; } = new();
    public DataTable<MeleeWeaponDataTable> MeleeTable { get; } = new();
    public DataTable<RangedWeaponDataTable> RangedTable { get; } = new();
    public DataTable<ItemDataTable> ItemTable { get; } = new();

    public bool IsLoaded { get; private set; }

    private readonly DataTableValidator validator = new();

    public IEnumerator LoadAll()
    {
        // yield return LoadTable("EnemyTable", EnemyTable);
        // yield return LoadTable("PlayerTable", PlayerTable);
        // yield return LoadTable("SackTable", SackTable);
        // yield return LoadTable("MeleeTable", MeleeTable);
        // yield return LoadTable("RangedTable", RangedTable);
        // yield return LoadTable("ItemTable", ItemTable);

        TextAsset[] textAssets = JsonDataLoader.LoadAll("JSON");
        foreach (TextAsset textAsset in textAssets)
        {
            yield return LoadTable(textAsset);
        }

        if (!validator.Validate())
        {
            Debug.LogError("DataTable validation failed");
        }

        IsLoaded = true;
    }

    private IEnumerator LoadTable(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("JSON file is null");
            yield break;
        }

        string json = textAsset.text;
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError($"JSON is empty: {textAsset.name}");
            yield break;
        }

        // tableName, records 꺼내기
        JObject root = JObject.Parse(json);
        string tableName = root["tableName"]?.ToString();
        JToken recordsToken = root["records"];

        if (string.IsNullOrEmpty(tableName) || recordsToken == null)
        {
            Debug.LogError($"tableName or records is missing: {textAsset.name}");
            yield break;
        }

        // DataManager에서 tableName과 같은 이름의 프로퍼티 찾기
        PropertyInfo tableProperty = GetType().GetProperty(tableName);
        if (tableProperty == null)
        {
            Debug.LogError($"Table property not found : {tableName}");
            yield break;
        }

        // DataTable<T> 타입인지 확인
        Type tableType = tableProperty.PropertyType;
        if (!tableType.IsGenericType || tableType.GetGenericTypeDefinition() != typeof(DataTable<>))
        {
            Debug.LogError($"{tableName} is not DataTable<T>");
            yield break;
        }

        // List<T> 타입 만들기
        Type dataType = tableType.GetGenericArguments()[0];
        Type listType = typeof(List<>).MakeGenericType(dataType);

        object records = recordsToken.ToObject(listType);
        if (records == null)
        {
            Debug.LogError($"JSON Deserialize failed: {textAsset.name}");
            yield break;
        }

        // Load 메서드 호출
        object table = tableProperty.GetValue(this);
        MethodInfo method = tableType.GetMethod("Load");

        if (method == null)
        {
            Debug.LogError($"Load method not found: {tableName}");
            yield break;
        }
        method.Invoke(table, new object[] { records });

        yield return null;
    }

    private IEnumerator LoadTable<T>(string fileName, DataTable<T> table) where T : IDataTable
    {
        yield return JsonDataLoader.LoadText(fileName, json =>
        {
            if (string.IsNullOrEmpty(json)) return;

            List<T> records = JsonConvert.DeserializeObject<List<T>>(json);

            if (records == null)
            {
                Debug.LogError($"JSON Deserialize failed: {fileName}");
                return;
            }

            table.Load(records);
        });
    }

    public IEnumerator WaitUntilLoaded()
    {
        while (!IsLoaded)
        {
            yield return null;
        }
    }
}
