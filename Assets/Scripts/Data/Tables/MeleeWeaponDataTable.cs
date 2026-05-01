public class MeleeWeaponDataTable : IDataTable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public MeleeLevel MeleeLevel { get; set; }
    public int AttackDamage { get; set; }
    public int TreeDamage { get; set; }

    public bool CanDrop { get; set; }
}