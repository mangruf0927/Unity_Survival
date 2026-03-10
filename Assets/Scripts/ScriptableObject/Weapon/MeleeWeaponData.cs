using UnityEngine;

public enum MeleeLevel { OLD, GOOD, STRONG }

[CreateAssetMenu]
public class MeleeWeaponData : ScriptableObject
{    
    [Header("무기 레벨")]
    public MeleeLevel level;

    [Header("공격력")]
    public int attackDamage;
}
