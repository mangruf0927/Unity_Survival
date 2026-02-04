using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private RangedWeaponData weaponData;

    [SerializeField] private Transform shootPosition;
    [SerializeField] private Vector3 target;

    [SerializeField] private float bulletSpeed;

    [SerializeField] GameObject projectile;

    public override void EnterAttack()
    {
    }

    public override void Attack()
    {
        GameObject bulletObj = Instantiate(projectile, shootPosition.position, shootPosition.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Fire(shootPosition.forward, bulletSpeed);
    }

    public override void ExitAttack()
    {
        
    }

}
