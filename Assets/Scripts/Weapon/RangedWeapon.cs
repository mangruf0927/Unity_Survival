using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private RangedWeaponData weaponData;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private Vector3 target;
    [SerializeField] GameObject projectile;

    public override void EnterAttack()
    {
    }

    public override void Attack()
    {
        GameObject bulletObj = Instantiate(projectile, shootPosition.position, shootPosition.rotation);
        
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetData(weaponData.attackDamage, weaponData.bulletSpeed);
            bullet.Fire(shootPosition.forward);
        }
    }

    public override void ExitAttack()
    {
        
    }

}
