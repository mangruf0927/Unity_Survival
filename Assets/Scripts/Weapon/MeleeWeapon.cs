using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private Collider hitCollider;

    private bool hasHit;

    private MeleeLevel meleeLevel;
    private int attackDamage;
    private int treeDamage;

    public MeleeLevel Level => meleeLevel;
    public override bool CanDrop => canDrop;

    private void Start()
    {
        MeleeWeaponData data = DataManager.Instance.MeleeTable.Get(ItemId);
        SetUp(data);
    }

    public void SetUp(MeleeWeaponData data)
    {
        itemName = data.Name;
        meleeLevel = data.MeleeLevel;
        attackDamage = data.AttackDamage;
        treeDamage = data.TreeDamage;
        canDrop = data.CanDrop;
    }

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
            if (ItemType != ToolType.AXE || treeDamage <= 0) return;

            hasHit = true;
            tree.TakeDamage(treeDamage);
            return;
        }

        if (!other.CompareTag("Enemy") || attackDamage <= 0) return;
        if (other.TryGetComponent<IDamageable>(out var enemy))
        {
            hasHit = true;
            enemy.TakeDamage(attackDamage);
        }
    }
}
