using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private BoxCollider hitCollider;

    public MeleeLevel Level => weaponData.level; 
    private int AttackDamage => weaponData.attackDamage;
    private bool hasHit;

    public override void Attack()
    {
        hasHit = false;
        hitCollider.enabled = true;
    }

    public override void ExitAttack()
    {
        hitCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit || AttackDamage <= 0) return;
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<IDamageable>(out var enemy))
        {
            hasHit = true;
            enemy.TakeDamage(AttackDamage);
        }
    }
}

