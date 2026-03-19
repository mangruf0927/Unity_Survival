using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private RangedWeaponData weaponData;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject projectile;
    [SerializeField] private ObjectPool pool;
    
    private int totalAmmo = 0;
    private int currentAmmo;

    public int MagSize => weaponData.magSize;
    public AmmoType type => weaponData.ammoType;

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

    public void SetTotalAmmo(int count) { totalAmmo = count; }

    public int NeedAmmo()
    {
        return MagSize - currentAmmo;
    }

    public void Reload(int amount)
    {
        if(amount <= 0) return;

        currentAmmo += amount;
        Debug.Log("[RangedWeapon Reload] : " + currentAmmo + "/" + totalAmmo);
    }
}
