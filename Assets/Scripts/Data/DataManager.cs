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
    public DataTable<CultistData> CultistTable { get; } = new();

    public bool IsLoaded { get; private set; }

    private readonly DataTableValidator validator = new();
    private readonly Dictionary<string, Action<JToken>> tableLoaderDictionary = new();

    private DataManager()
    {
        tableLoaderDictionary.Add("EnemyTable", records => EnemyTable.LoadFromToken(records));
        tableLoaderDictionary.Add("PlayerTable", records => PlayerTable.LoadFromToken(records));
        tableLoaderDictionary.Add("SackTable", records => SackTable.LoadFromToken(records));
        tableLoaderDictionary.Add("MeleeTable", records => MeleeTable.LoadFromToken(records));
        tableLoaderDictionary.Add("RangedTable", records => RangedTable.LoadFromToken(records));
        tableLoaderDictionary.Add("ItemTable", records => ItemTable.LoadFromToken(records));
        tableLoaderDictionary.Add("CampFireTable", records => CampFireTable.LoadFromToken(records));
        tableLoaderDictionary.Add("CultistTable", records => CultistTable.LoadFromToken(records));
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
            throw new InvalidOperationException("DataTable validation failed");
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