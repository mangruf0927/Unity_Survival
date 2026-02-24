using UnityEngine;

[CreateAssetMenu]
public class RangedWeaponData : ScriptableObject
{
    [Header("공격력")]
    public int attackDamage;

    [Header("최대 장전 수")]
    public int magSize;

    [Header("총알 속도")]
    public int bulletSpeed;
}
