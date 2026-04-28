using System.Collections.Generic;
using UnityEngine;

public class DataTable<T> where T : IDataTable
{
    private readonly Dictionary<int, T> _table = new();
    public IReadOnlyDictionary<int, T> All => _table;

    public void Load(IEnumerable<T> records)
    {
        _table.Clear();
        foreach (var record in records)
        {
            if (_table.ContainsKey(record.Id))
            {
                Debug.LogError($"Duplicate id: {typeof(T).Name} - {record.Id}");
                continue;
            }
            _table.Add(record.Id, record);
        }
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
