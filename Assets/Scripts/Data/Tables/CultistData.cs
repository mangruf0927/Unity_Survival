using System;
using UnityEngine;

public class CultistData : IGameData, IValidatable
{
    public int Id { get; set; }

    public int MaxHp { get; set; }
    public float ScanRange { get; set; }
    public float MaxRaidCenterDistance { get; set; }
    public float ReturnDistance { get; set; }
    public float ReturnSearchRange { get; set; }
    public float AlertDuration { get; set; }
    public PoolTypeEnums CultistType { get; set; }

    public bool Validate()
    {
        if (Id <= 0)
        {
            Debug.LogError($"Cultist Id is invalid. Id: {Id}");
            return false;
        }

        if (MaxHp <= 0)
        {
            Debug.LogError($"Cultist MaxHp is invalid. Id: {Id}, MaxHp: {MaxHp}");
            return false;
        }

        if (ScanRange < 0f ||
            MaxRaidCenterDistance < 0f ||
            ReturnDistance < 0f ||
            ReturnSearchRange < 0f)
        {
            Debug.LogError($"Cultist range value is invalid. Id: {Id}");
            return false;
        }

        if (AlertDuration < 0f)
        {
            Debug.LogError($"Cultist AlertDuration is invalid. Id: {Id}, AlertDuration: {AlertDuration}");
            return false;
        }

        if (!Enum.IsDefined(typeof(PoolTypeEnums), CultistType))
        {
            Debug.LogError($"CultistType is invalid. Id: {Id}, CultistType: {CultistType}");
            return false;
        }

        return true;
    }
}
