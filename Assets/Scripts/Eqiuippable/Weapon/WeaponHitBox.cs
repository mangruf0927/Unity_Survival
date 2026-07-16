using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    private MeleeWeapon weapon;

    private void Awake()
    {
        weapon = GetComponentInParent<MeleeWeapon>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (weapon.HasHit) return;

        TreeObject tree = other.GetComponentInParent<TreeObject>();
        if (tree != null)
        {
            if (weapon.ItemType != ToolType.AXE || weapon.TreeDamage <= 0) return;

            weapon.SetHasHit(true);
            tree.TakeDamage(weapon.TreeDamage);
            return;
        }

        if (!other.CompareTag("Enemy") || weapon.AttackDamage <= 0) return;
        if (other.GetComponentInParent<IDamageable>() is IDamageable enemy)
        {
            weapon.SetHasHit(true);
            enemy.TakeDamage(weapon.AttackDamage);
        }
    }
}
