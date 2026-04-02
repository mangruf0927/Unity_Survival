using UnityEngine;

[CreateAssetMenu]
public class MeleeWeaponData : ScriptableObject
{
    [Header("무기 레벨")]
    public MeleeLevel level;

    [Header("공격력")]
    public int attackDamage;

    [Header("나무 피해량")]
    public int treeDamage;
}
