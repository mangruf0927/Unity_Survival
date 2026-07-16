using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataTableValidator
{
    public bool Validate()
    {
        bool isValid = true;

        isValid &= ValidateSackTable();

        return isValid;
    }

    private bool ValidateSackTable()
    {
        Dictionary<int, SackData> sackLevelDictionary = new();

        foreach (SackData sack in DataManager.Instance.SackTable.All.Values)
        {
            // 중복 등록 방지
            if (sackLevelDictionary.ContainsKey(sack.Level))
            {
                Debug.LogError($"Duplicate SackLevel found. Level: {sack.Level}");
                return false;
            }
            sackLevelDictionary.Add(sack.Level, sack);
        }

        if (sackLevelDictionary.Count == 0)
        {
            Debug.LogError("Sack data is empty");
            return false;
        }

        // 레벨 누락 검사
        int maxLevel = sackLevelDictionary.Keys.Max();
        for (int i = 1; i <= maxLevel; i++)
        {
            if (sackLevelDictionary.ContainsKey(i)) continue;

            Debug.LogError($"Sack data is missing. Level: {i}");
            return false;
        }

        // Capacity 증가 규칙 검사
        for (int i = 1; i < maxLevel; i++)
        {
            SackData cur = sackLevelDictionary[i];
            SackData next = sackLevelDictionary[i + 1];

            if (cur.Capacity > next.Capacity)
            {
                Debug.LogError($"Sack Capacity rule is invalid. {cur.Level} : {cur.Capacity}, {next.Level} : {next.Capacity}");
                return false;
            }
        }
        return true;
    }
}
