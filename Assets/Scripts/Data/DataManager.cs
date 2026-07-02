using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;

public class DataManager
{
    public static DataManager Instance { get; } = new();

    public DataTable<EnemyData> EnemyTable { get; } = new();
    public DataTable<PlayerData> PlayerTable { get; } = new();
    public DataTable<SackData> SackTable { get; } = new();
    public DataTable<MeleeWeaponData> MeleeTable { get; } = new();
    public DataTable<RangedWeaponData> RangedTable { get; } = new();
    public DataTable<ItemData> ItemTable { get; } = new();
    public DataTable<CampFireData> CampFireTable { get; } = new();

    public bool IsLoaded { get; private set; }

    private readonly DataTableValidator validator = new();
    private readonly Dictionary<string, Action<JToken>> tableLoaderDictionary = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        Instance.LoadAll();
    }

    private DataManager()
    {
        tableLoaderDictionary.Add("EnemyTable", records => EnemyTable.LoadFromToken(records));
        tableLoaderDictionary.Add("PlayerTable", records => PlayerTable.LoadFromToken(records));
        tableLoaderDictionary.Add("SackTable", records => SackTable.LoadFromToken(records));
        tableLoaderDictionary.Add("MeleeTable", records => MeleeTable.LoadFromToken(records));
        tableLoaderDictionary.Add("RangedTable", records => RangedTable.LoadFromToken(records));
        tableLoaderDictionary.Add("ItemTable", records => ItemTable.LoadFromToken(records));
        tableLoaderDictionary.Add("CampFireTable", records => CampFireTable.LoadFromToken(records));
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

        if (string.IsNullOrEmpty(textAsset.text))
        {
            Debug.LogError($"JSON is empty: {textAsset.name}");
            return;
        }

        BaseGameData baseData = JsonConvert.DeserializeObject<BaseGameData>(textAsset.text);

        if (baseData == null)
        {
            Debug.LogError($"JSON Deserialize failed : {textAsset.name}");
            return;
        }

        if (string.IsNullOrEmpty(baseData.tableName) || baseData.records == null)
        {
            Debug.LogError($"tableName or records is missing: {textAsset.name}");
            return;
        }

        if (!tableLoaderDictionary.TryGetValue(baseData.tableName, out Action<JToken> loader))
        {
            Debug.LogError($"Table loader not fount : {baseData.tableName}");
            return;
        }

        loader.Invoke(baseData.records);
    }
}

// 데이터 매니저 로드를 RunTimeInitializeOnLoadMethod 기반으로 하면 좋을 것 같다.
// 씬 로드전에 데이터 매니저의 세팅을 보장할 수 있기 때문에 WaitUntilLoaded라는 보일러 플레이트가 없어짐

/*
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

string jsonText = @"{
  ""TableName"" : ""EquipmentTable"",
  ""Records"": [
    {
      ""Id"": 1,
      ""Name"": ""칼""
    },
    {
      ""Id"": 2,
      ""Name"": ""긴칼""
    },
    {
      ""Id"": 3,
      ""Name"": ""짧은칼""
    }
  ]
}";


var json = JsonConvert.DeserializeObject<BaseGameData>(jsonText);
Console.WriteLine(json.TableName);

// json

public class BaseGameData
{
    public string TableName { get; set; }
    public List<EquipmentData>  Records { get; set; }
}


public class EquipmentData : IGameData
{
    public int Id { get; set; }
    public string Name { get; set; }
}

interface IGameData
{
    public int Id { get; set; }
}

*/
