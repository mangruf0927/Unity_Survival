using UnityEngine;

public enum WeaponTypeEnums { MELEE, RANGED }

public abstract class Weapon : MonoBehaviour
{
    public WeaponTypeEnums weaponType;

    public Vector3 aimPos;
    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

    public abstract void EnterAttack();
    public abstract void Attack();
    public abstract void ExitAttack();
}
