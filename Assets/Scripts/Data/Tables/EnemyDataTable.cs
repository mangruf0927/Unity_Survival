using UnityEngine;

public class EnemyDataTable : IDataTable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxHp { get; set; }
    public int AttackDamage { get; set; }
}
