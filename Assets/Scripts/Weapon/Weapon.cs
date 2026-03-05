using Unity.Burst.Intrinsics;
using UnityEngine;

public enum WeaponTypeEnums { MELEE, RANGED }

public abstract class Weapon : EquippableItem
{
    public WeaponTypeEnums weaponType;

    public bool canDrop;
    public Vector3 aimPos;
    public override bool CanDrop => canDrop;

    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

    public override void OnEquip(PlayerController player)
    {
        player.currentWeapon = this;
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        ExitAttack();
        player.currentWeapon = null;
        gameObject.SetActive(false);
    }

    public abstract void EnterAttack();
    public abstract void Attack();
    public abstract void ExitAttack();
}
