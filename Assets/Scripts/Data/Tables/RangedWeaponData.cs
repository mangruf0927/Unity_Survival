using System;
using UnityEngine;

public class RangedWeaponData : IGameData, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public AmmoType Type { get; set; }
    public int AttackDamage { get; set; }
    public int MagSize { get; set; }
    public float BulletSpeed { get; set; }

    public bool CanDrop { get; set; }

    public bool Validate()
    {
        if (Id <= 2100 || Id > 2200)
        {
            Debug.LogError($"Ranged Id is invalid: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Ranged Name is empty. Id: {Id}");
            return false;
        }

        if (AttackDamage < 0)
        {
            Debug.LogError($"Ranged Damage is invalid. Id: {Id}, AttackDamage: {AttackDamage}");
            return false;
        }

        if (MagSize <= 0)
        {
            Debug.LogError($"Ranged MagSize is invalid. Id: {Id}, MagSize: {MagSize}");
            return false;
        }

        if (BulletSpeed <= 0)
        {
            Debug.LogError($"Ranged BulletSpeed is invalid. Id: {Id}, BulletSpeed: {BulletSpeed}");
            return false;
        }

        if (!Enum.IsDefined(typeof(AmmoType), Type))
        {
            Debug.LogError($"Ranged is invalid. Id: {Id}, AmmoType: {Type} ");
            return false;
        }

        return true;
    }
}

