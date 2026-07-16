using System;
using UnityEngine;

public class SackData : IGameData, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int GroupId { get; set; }
    public int Level { get; set; }

    public int Capacity { get; set; }
    public bool CanDrop { get; set; }

    public bool Validate()
    {
        if (Id <= 2200 || Id > 2300)
        {
            Debug.LogError($"Sack Id is invalid. Id: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Sack Name is empty. Id: {Id}");
            return false;
        }

        if (GroupId <= 0)
        {
            Debug.LogError($"Sack GroupId is invalid. Id: {Id}, GroupId: {GroupId}");
            return false;
        }

        if (Level <= 0)
        {
            Debug.LogError($"SackLevel is invalid. Id: {Id}, Level: {Level} ");
            return false;
        }

        if (Capacity <= 0)
        {
            Debug.LogError($"Sack Damage is invalid. Id: {Id}, Capacity: {Capacity}");
            return false;
        }

        return true;
    }
}