using UnityEngine;

public abstract class CultistWeapon : MonoBehaviour
{
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCoolTime;
    [SerializeField] private Transform attachPoint;

    public float AttackRange => attackRange;
    public int AttackDamage => attackDamage;
    public float AttackCoolTime => attackCoolTime;
    public Transform AttachPoint => attachPoint;
    public virtual string AttackTrigger => "MeleeAttack";
    public virtual string AttackStateName => "attack_melee";

    public abstract void Attack(Transform target);
    public virtual void EndAttack() { }
}
