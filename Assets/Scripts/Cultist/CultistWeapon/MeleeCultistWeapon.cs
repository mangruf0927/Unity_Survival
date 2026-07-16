using UnityEngine;

public class MeleeCultistWeapon : CultistWeapon
{
    [SerializeField] private Collider hitCollider;

    private bool hasHit;

    private void Awake()
    {
        hitCollider.enabled = false;
    }

    public override void Attack(Transform target)
    {
        hasHit = false;
        hitCollider.enabled = true;
    }

    public override void EndAttack()
    {
        hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit || other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player") && other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(AttackDamage);
            hasHit = true;
        }
    }
}
