using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private RangedWeaponData weaponData;
    [SerializeField] private Transform shootPosition;

    [SerializeField] private GameObject projectile;
    [SerializeField] private ObjectPool pool;
    [SerializeField] private int totalAmmo;

    private int MagSize => weaponData.magSize;
    private int currentAmmo;

    private Vector3 aimPos;
    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

    public override void Attack()
    {
        if(currentAmmo <= 0) 
        {
            Debug.Log("[탄약 없음] : " + currentAmmo + "/" + totalAmmo);
            return;
        }

        GameObject bulletObj = pool.GetFromPool(projectile, PoolTypeEnums.BULLET);
        bulletObj.transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);
        
        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetData(weaponData.attackDamage, weaponData.bulletSpeed, pool);
            bullet.Fire(aimPos - shootPosition.position);

            currentAmmo--;
            Debug.Log("[발사] : " + currentAmmo + "/" + totalAmmo);
        }
    }

    public override void ExitAttack()
    {
    }

    public void Reload()
    {
        if(currentAmmo >= MagSize || totalAmmo <= 0) return;

        int need = MagSize - currentAmmo;
        int reload = Mathf.Min(need, totalAmmo);

        currentAmmo += reload;
        totalAmmo -= reload;

        Debug.Log("[재장전] : " + currentAmmo + "/" + totalAmmo);
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }
}
