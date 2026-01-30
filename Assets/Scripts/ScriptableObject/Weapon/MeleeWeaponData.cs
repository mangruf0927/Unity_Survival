using UnityEngine;

public enum MeleeWeaponEnums { AXE, SPEAR }

[CreateAssetMenu]
public class MeleeWeaponData : ScriptableObject
{
    [Header("무기 종류")]
    public MeleeWeaponEnums weaponType;

    [Header("공격력")]
    public int attackDamage;
}
