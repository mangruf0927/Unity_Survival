using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon, ISubject
{
    [SerializeField] private Transform shootPosition;

    private readonly List<IObserver> ObserverList = new();

    private int totalAmmo = 0;
    private int currentAmmo;

    private AmmoType ammoType;
    private int attackDamage;
    private int magSize;
    private float bulletSpeed;

    public int CurrentAmmo => currentAmmo;
    public int TotalAmmo => totalAmmo;
    public int MagSize => magSize;
    public AmmoType Type => ammoType;
    public override bool CanDrop => canDrop;

    private Vector3 aimPos;
    public void SetAimPoint(Vector3 pos) { aimPos = pos; }

    private void Awake()
    {
        RangedWeaponData data = DataManager.Instance.RangedTable.Get(ItemId);
        SetUp(data);
    }

    public void SetUp(RangedWeaponData data)
    {
        itemName = data.Name;
        ammoType = data.Type;
        attackDamage = data.AttackDamage;
        magSize = data.MagSize;
        bulletSpeed = data.BulletSpeed;
        canDrop = data.CanDrop;

        NotifyObservers();
    }

    public override void Attack()
    {
        if (currentAmmo <= 0) return;

        GameObject bulletObj = ObjectPool.Instance.GetFromPool(PoolTypeEnums.BULLET);
        bulletObj.transform.SetPositionAndRotation(shootPosition.position, shootPosition.rotation);

        if (bulletObj.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetData(attackDamage, bulletSpeed);
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

    public void SetCurrentAmmo(int count)
    {
        currentAmmo = magSize > 0 ? Mathf.Clamp(count, 0, magSize) : Mathf.Max(0, count);
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
