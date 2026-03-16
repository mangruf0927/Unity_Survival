using UnityEngine;

public abstract class Weapon : EquippableItem
{
    [SerializeField] private bool canDrop;
    public override bool CanDrop => canDrop;
    
    private Vector3 aimPos;
    protected Vector3 AimPos => aimPos;

    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

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
