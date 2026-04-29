using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager
{
    public static DataManager Instance { get; } = new();

    public DataTable<EnemyDataTable> EnemyTable { get; } = new();
    public DataTable<PlayerDataTable> PlayerTable { get; } = new();

    public bool IsLoaded { get; private set; }
    public IEnumerator LoadAll()
    {
        yield return LoadTable("EnemyTable", EnemyTable);
        yield return LoadTable("PlayerTable", PlayerTable);
        IsLoaded = true;
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
}
