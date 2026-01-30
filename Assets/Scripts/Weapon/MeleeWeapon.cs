using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private BoxCollider boxCollider;

    private bool hasHit;

    private void Awake()
    {
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }

    public void BeginAttack()
    {
        hasHit = false;
        boxCollider.enabled = true;
    }

    public void EndAttack()
    {
        boxCollider.enabled = false;
        hasHit = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(hasHit) return;
        if (weaponData.attackDamage <= 0) return;
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<IDamageable>();
        hasHit = true;
        enemy.TakeDamage(weaponData.attackDamage);
    }
}
