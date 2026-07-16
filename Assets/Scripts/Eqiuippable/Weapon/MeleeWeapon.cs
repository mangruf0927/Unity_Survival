using UnityEngine;

public class MeleeWeapon : Weapon, IUpgradeable
{
    [SerializeField] private Collider hitCollider;

    private bool hasHit;
    private int groupId;
    private int meleeLevel;
    private int attackDamage;
    private int treeDamage;

    public int GroupId => groupId;
    public int Level => meleeLevel;
    public override bool CanDrop => canDrop;

    public bool HasHit => hasHit;
    public int AttackDamage => attackDamage;
    public int TreeDamage => treeDamage;


    private void Awake()
    {
        MeleeWeaponData data = DataManager.Instance.MeleeTable.Get(ItemId);
        SetUp(data);
    }

    public void SetUp(MeleeWeaponData data)
    {
        itemName = data.Name;
        groupId = data.GroupId;
        meleeLevel = data.Level;
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

    public void SetHasHit(bool hasHit)
    {
        this.hasHit = hasHit;
    }
}
