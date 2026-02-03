using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    // [SerializeField] private WeaponData weaponData;

    [SerializeField] private Transform shootPosition;
    [SerializeField] private Vector3 target;

    [SerializeField] private float bulletSpeed;

    [SerializeField] GameObject projectile;

    public void Shoot()
    {
        GameObject bulletObj = Instantiate(projectile, shootPosition.position, shootPosition.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Fire(shootPosition.forward, bulletSpeed);
    }
}
