using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon, ISubject
{
    [SerializeField] private RangedWeaponData weaponData;
    [SerializeField] private Transform shootPosition;

    private readonly List<IObserver> ObserverList = new();

    private int totalAmmo = 0;
    private int currentAmmo;

    public int CurrentAmmo => currentAmmo;
    public int TotalAmmo => totalAmmo;
    public int MagSize => weaponData.magSize;
    public AmmoType Type => weaponData.ammoType;

    private Vector3 aimPos;
    public void SetAimPoint(Vector3 pos) { aimPos = pos; }

    public override void Attack()
    {
        if (currentAmmo <= 0) return;

        GameObject bulletObj = ObjectPool.Instance.GetFromPool(PoolTypeEnums.BULLET);
        bulletObj.transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);

        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetData(weaponData.attackDamage, weaponData.bulletSpeed);
            bullet.Fire(aimPos - shootPosition.position);

            currentAmmo--;
            NotifyObservers();
        }
    }

    public override void ExitAttack()
    {
    }

    public void SetTotalAmmo(int count)
    {
        totalAmmo = count;
        NotifyObservers();
    }

    public int NeedAmmo()
    {
        return MagSize - currentAmmo;
    }

    public void Reload(int amount)
    {
        if (amount <= 0) return;

        currentAmmo += amount;
        NotifyObservers();
    }

    public void AddObserver(IObserver observer)
    {
        ObserverList.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        ObserverList.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in ObserverList)
        {
            observer.Notify();
        }
    }
}
