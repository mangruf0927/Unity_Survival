public class RangedWeaponDataTable : IDataTable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public AmmoType Type { get; set; }
    public int AttackDamage { get; set; }
    public int MagSize { get; set; }
    public float BulletSpeed { get; set; }

    public bool CanDrop { get; set; }
}

