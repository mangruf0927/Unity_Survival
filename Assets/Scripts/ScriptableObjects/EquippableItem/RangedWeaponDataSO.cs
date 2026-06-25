using UnityEngine;

[CreateAssetMenu]
public class RangedWeaponDataSO : ScriptableObject
{
    [Header("총 타입")]
    public AmmoType ammoType;

    [Header("공격력")]
    public int attackDamage;

    [Header("최대 장전 수")]
    public int magSize;

    [Header("총알 속도")]
    public int bulletSpeed;
}
