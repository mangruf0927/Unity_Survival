using UnityEngine;

public class CultistWeapon : MonoBehaviour
{
    [SerializeField] private Collider hitCollider;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCoolTime;

    public float AttackRange => attackRange;
    public int AttackDamage => attackDamage;
    public float AttackCoolTime => attackCoolTime;

    private bool hasHit;

    private void Awake()
    {
        hitCollider.enabled = false;
    }

    public void Attack()
    {
        hasHit = false;
        hitCollider.enabled = true;
    }

    public void EndAttack()
    {
        hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<IDamageable>(out var damageable)) return;

        hasHit = true;
        damageable.TakeDamage(attackDamage);
    }
}
