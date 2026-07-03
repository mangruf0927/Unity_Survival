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

        Vector3 direction = target.position - shootPosition.position;
        if (direction == Vector3.zero) direction = shootPosition.forward;

        CultistBullet bullet = Instantiate(bulletPrefab, shootPosition.position, Quaternion.LookRotation(direction, Vector3.up));
        bullet.SetData(AttackDamage, bulletSpeed);
        bullet.Fire(direction);
    }
}
