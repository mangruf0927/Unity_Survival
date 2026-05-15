using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DataTable<T> where T : IGameData
{
    private readonly Dictionary<int, T> _table = new();
    public IReadOnlyDictionary<int, T> All => _table;

    public void Load(IEnumerable<T> records)
    {
        _table.Clear();

        foreach (var record in records)
        {
            if (record is IValidatable validatable && !validatable.Validate())
            {
                Debug.LogError($"Invalid data skipped: {typeof(T).Name} - Id: {record.Id}");
                continue;
            }

            if (_table.ContainsKey(record.Id))
            {
                Debug.LogError($"Duplicate id: {typeof(T).Name} - {record.Id}");
                continue;
            }
            _table.Add(record.Id, record);
        }
    }

    public void LoadFromToken(JToken records)
    {
        List<T> recordList = records.ToObject<List<T>>();

        if (recordList == null)
        {
            Debug.LogError($"JSON Deserialize failed : {typeof(T).Name}");
            return;
        }
        Load(recordList);
    }

    public T Get(int id)
    {
        if (!_table.TryGetValue(id, out var result))
        {
            Debug.LogError($"Not found id: {typeof(T).Name} - {id}");
            return default;
        }
        return result;
    }

    public bool TryGet(int id, out T result)
    {
        return _table.TryGetValue(id, out result);
    }
}
