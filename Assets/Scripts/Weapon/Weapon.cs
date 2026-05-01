using UnityEngine;

public abstract class Weapon : EquippableItem
{

    public override void OnEquip(PlayerController player)
    {
        player.SetWeapon(this);
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        ExitAttack();
        player.SetWeapon(null);
        gameObject.SetActive(false);
    }

    public abstract void Attack();
    public abstract void ExitAttack();
}
