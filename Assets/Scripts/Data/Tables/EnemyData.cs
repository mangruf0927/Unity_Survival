using System;
using UnityEngine;

public class EnemyData : IGameData, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int MaxHp { get; set; }
    public int AttackDamage { get; set; }
    public bool CanChase { get; set; }
    public float ScanRange { get; set; }
    public float PatrolRange { get; set; }
    public float AlertDuration { get; set; }
    public PoolTypeEnums EnemyType { get; set; }

    public bool Validate()
    {
        if (Id <= 1100 || Id > 1200)
        {
            Debug.LogError($"Enemy Id is invalid. Id: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Enemy Name is empty. Id: {Id}");
            return false;
        }

        if (MaxHp <= 0)
        {
            Debug.LogError($"Enemy MaxHp is invalid. Id: {Id}, MaxHp: {MaxHp}");
            return false;
        }

        if (AttackDamage < 0)
        {
            Debug.LogError($"Enemy AttackDamage is invalid. Id: {Id}, AttackDamage: {AttackDamage}");
            return false;
        }

        if (ScanRange < 0 || PatrolRange < 0)
        {
            Debug.LogError($"Enemy Range is invalid. Id: {Id}, ScanRange: {ScanRange}, PatrolRange: {PatrolRange}");
            return false;
        }

        if (AlertDuration < 0)
        {
            Debug.LogError($"AlertDuration is invalid. Id: {Id}, AlertDuration: {AlertDuration}");
            return false;
        }

        if (!Enum.IsDefined(typeof(PoolTypeEnums), EnemyType))
        {
            Debug.LogError($"EnemyType is invalid. Id: {Id}, EnemyType: {EnemyType}");
            return false;
        }

        return true;
    }
}