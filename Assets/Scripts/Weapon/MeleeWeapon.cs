using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private Collider hitCollider;

    public MeleeLevel Level => weaponData.level;
    private int AttackDamage => weaponData.attackDamage;
    private int TreeDamage => weaponData.treeDamage;

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
        if (hasHit) return;

        TreeObject tree = other.GetComponentInParent<TreeObject>();
        if (tree != null)
        {
            if (ItemType != ToolType.AXE || TreeDamage <= 0) return;

            hasHit = true;
            tree.TakeDamage(TreeDamage);
            return;
        }

        if (!other.CompareTag("Enemy") || AttackDamage <= 0) return;
        if (other.TryGetComponent<IDamageable>(out var enemy))
        {
            hasHit = true;
            enemy.TakeDamage(AttackDamage);
        }
    }
}

