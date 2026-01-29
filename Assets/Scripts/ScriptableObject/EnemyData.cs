using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    [Header("체력")]
    public int maxHP;

    [Header("공격력")]
    public int attackDamage;
    
    [Header("적 탐지 범위")]
    public float scanRange;

    [Header("플레이어 추적 가능")]
    public bool canChase;

    [Header("순찰 범위")]
    public float patrolRange;
}
