public class PlayerDataTable : IDataTable
{
    public int Id { get; set; }

    public int MaxHp { get; set; }
    public float MoveSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float JumpForce { get; set; }
    public float RotateSpeed { get; set; }
}
