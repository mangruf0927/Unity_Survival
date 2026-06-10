using UnityEngine;

public class RangedCultistWeapon : CultistWeapon
{
    [SerializeField] private CultistBullet bulletPrefab;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private float bulletSpeed;

    public override string AttackTrigger => "RangedAttack";
    public override string AttackStateName => "attack_ranged";

    public override void Attack(Transform target)
    {
        if (bulletPrefab == null || shootPosition == null || target == null) return;

        CultistBullet bullet = Instantiate(bulletPrefab, shootPosition.position, shootPosition.rotation);
        bullet.SetData(AttackDamage, bulletSpeed);
        bullet.Fire(target.position - shootPosition.position);
    }
}
