using System;
using System.Collections.Generic;
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
        Dictionary<SackLevel, SackData> sackLevelDictionary = new();

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

        SackLevel[] levelArray = (SackLevel[])Enum.GetValues(typeof(SackLevel));

        foreach (SackLevel level in levelArray)
        {
            // 데이터 누락 방지 
            if (!sackLevelDictionary.ContainsKey(level))
            {
                Debug.LogError($"Sack data is missing. Level : {level}");
                return false;
            }
        }

        for (int i = 0; i < levelArray.Length - 1; i++)
        {
            // Capacity 증가 규칙 검사
            SackData cur = sackLevelDictionary[levelArray[i]];
            SackData next = sackLevelDictionary[levelArray[i + 1]];

            if (cur.Capacity > next.Capacity)
            {
                Debug.LogError($"Sack Capacity rule is invalid. {cur.Level} : {cur.Capacity}, {next.Level} : {next.Capacity}");
                return false;
            }
        }

        return true;
    }
}
