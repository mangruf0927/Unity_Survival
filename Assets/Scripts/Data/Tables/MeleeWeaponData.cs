using System;
using UnityEngine;

public class MeleeWeaponData : IGameData, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public MeleeLevel MeleeLevel { get; set; }
    public int AttackDamage { get; set; }
    public int TreeDamage { get; set; }

    public bool CanDrop { get; set; }

    public bool Validate()
    {
        if (Id <= 2000 || Id > 2100)
        {
            Debug.LogError($"Melee Id is invalid. Id: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Melee Name is empty. Id: {Id}");
            return false;
        }

        if (AttackDamage < 0 || TreeDamage < 0)
        {
            Debug.LogError($"Melee Damage is invalid. Id: {Id}, AttackDamage: {AttackDamage}, TreeDamage: {TreeDamage}");
            return false;
        }

        if (!Enum.IsDefined(typeof(MeleeLevel), MeleeLevel))
        {
            Debug.LogError($"MeleeLevel is invalid. Id: {Id}, MeleeLevel: {MeleeLevel} ");
            return false;
        }

        return true;
    }
}