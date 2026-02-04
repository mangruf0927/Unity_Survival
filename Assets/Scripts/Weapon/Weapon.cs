using UnityEngine;

public enum WeaponTypeEnums { MELEE, RANGED }

public abstract class Weapon : MonoBehaviour
{
    public WeaponTypeEnums weaponType;

    public abstract void EnterAttack();
    public abstract void Attack();
    public abstract void ExitAttack();
}
