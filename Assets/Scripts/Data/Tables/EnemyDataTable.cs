using UnityEngine;

public class EnemyDataTable : IDataTable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int MaxHp { get; set; }
    public int AttackDamage { get; set; }
    public bool CanChase { get; set; }
    public float ScanRange { get; set; }
    public float PatrolRange { get; set; }
    public PoolTypeEnums EnemyType { get; set; }
}