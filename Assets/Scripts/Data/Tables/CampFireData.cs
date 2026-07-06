using System.Collections.Generic;
using UnityEngine;

public class CampFireData : IGameData, IValidatable
{
    public int Id { get; set; }

    public int MaxLevel { get; set; }
    public float MaxFuel { get; set; }
    public float FuelAfterLevelUp { get; set; }

    public int DecreaseAmount { get; set; }
    public List<float> DecreaseTimeList { get; set; }

    public float LevelUpDelay { get; set; }
    public float WarningThreshold { get; set; }

    public bool Validate()
    {
        if (Id <= 3000 || Id > 3100)
        {
            Debug.LogError($"CampFire Id is invalid. Id: {Id}");
            return false;
        }

        if (MaxLevel <= 0)
        {
            Debug.LogError($"CampFire MaxLevel is invalid. Id: {Id}, MaxLevel: {MaxLevel}");
            return false;
        }

        if (MaxFuel <= 0f)
        {
            Debug.LogError($"CampFire MaxFuel is invalid. Id: {Id}, MaxFuel: {MaxFuel}");
            return false;
        }

        if (FuelAfterLevelUp < 0f || FuelAfterLevelUp > MaxFuel)
        {
            Debug.LogError($"CampFire FuelAfterLevelUp is invalid. Id: {Id}, FuelAfterLevelUp: {FuelAfterLevelUp}");
            return false;
        }

        if (DecreaseAmount <= 0)
        {
            Debug.LogError($"CampFire DecreaseAmount is invalid. Id: {Id}, DecreaseAmount: {DecreaseAmount}");
            return false;
        }

        if (DecreaseTimeList == null || DecreaseTimeList.Count == 0)
        {
            Debug.LogError($"CampFire DecreaseTimeList is missing. Id: {Id}");
            return false;
        }

        foreach (float decreaseTime in DecreaseTimeList)
        {
            if (decreaseTime <= 0f)
            {
                Debug.LogError($"CampFire DecreaseTime is invalid. Id: {Id}, DecreaseTime: {decreaseTime}");
                return false;
            }
        }

        if (LevelUpDelay < 0f)
        {
            Debug.LogError($"CampFire LevelUpDelay is invalid. Id: {Id}, LevelUpDelay: {LevelUpDelay}");
            return false;
        }

        if (WarningThreshold < 0f)
        {
            Debug.LogError($"CampFire WarningThreshold is invalid. Id: {Id}, WarningThreshold: {WarningThreshold}");
            return false;
        }

        return true;
    }

    public float GetDecreaseTime(int level)
    {
        if (DecreaseTimeList == null || DecreaseTimeList.Count == 0) return 1f;

        int index = Mathf.Clamp(level - 1, 0, DecreaseTimeList.Count - 1);
        return DecreaseTimeList[index];
    }
}
