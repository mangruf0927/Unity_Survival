using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private BoxCollider boxCollider;

    private bool hasHit;

    private void Awake()
    {
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }

    public override void EnterAttack()
    {
        hasHit = false;
        boxCollider.enabled = true;
    }

    public override void Attack()
    {
        
    }

    public override void ExitAttack()
    {
        boxCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(hasHit) return;
        if (weaponData.attackDamage <= 0) return;
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<IDamageable>(out var enemy))
        {
            hasHit = true;
            enemy.TakeDamage(weaponData.attackDamage);
        }
    }
}
