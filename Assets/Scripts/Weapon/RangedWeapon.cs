using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private RangedWeaponData weaponData;
    [SerializeField] private Transform shootPosition;

    [SerializeField] private GameObject projectile;
    [SerializeField] private ObjectPool pool;

    public override void EnterAttack()
    {
    }

    public override void Attack()
    {
        GameObject bulletObj = pool.GetFromPool(projectile, PoolTypeEnums.BULLET);
        bulletObj.transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
        
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetData(weaponData.attackDamage, weaponData.bulletSpeed, pool);
            bullet.Fire(aimPos - shootPosition.position);
        }
    }

    public override void ExitAttack()
    {
        
    }

}
