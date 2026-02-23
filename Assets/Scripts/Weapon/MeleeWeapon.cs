using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private BoxCollider hitCollider;

    private bool hasHit;

    private void Awake()
    {
        hitCollider.isTrigger = true;
        hitCollider.enabled = false;
    }

    public override void EnterAttack()
    {
        hasHit = false;
        hitCollider.enabled = true;
    }

    public override void Attack()
    {
        
    }

    public override void ExitAttack()
    {
        hitCollider.enabled = false;
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

