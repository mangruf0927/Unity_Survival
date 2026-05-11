using System;
using UnityEngine;

public class SackData : IDataTable, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public SackLevel Level { get; set; }
    public int Capacity { get; set; }

    public bool CanDrop { get; set; }

    public bool Validate()
    {
        if (Id <= 0)
        {
            Debug.LogError($"Sack Id is invalid. Id: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Sack Name is empty. Id: {Id}");
            return false;
        }

        if (Capacity <= 0)
        {
            Debug.LogError($"Sack Damage is invalid. Id: {Id}, Capacity: {Capacity}");
            return false;
        }

        if (!Enum.IsDefined(typeof(SackLevel), Level))
        {
            Debug.LogError($"Sack is invalid. Id: {Id}, SackLevel: {Level}");
            return false;
        }

        return true;
    }
}