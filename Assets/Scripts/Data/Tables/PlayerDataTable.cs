using UnityEngine;

public class PlayerDataTable : IDataTable, IValidatable
{
    public int Id { get; set; }

    public int MaxHp { get; set; }
    public float MoveSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float JumpForce { get; set; }
    public float RotateSpeed { get; set; }

    public bool Validate()
    {
        if (Id <= 0)
        {
            Debug.LogError($"Player Id is invalid. Id: {Id}");
            return false;
        }

        if (MaxHp <= 0)
        {
            Debug.LogError($"Player MaxHp is invalid. MaxHp: {MaxHp}");
            return false;
        }

        if (MoveSpeed <= 0)
        {
            Debug.LogError($"Player MoveSpeed is invalid. MoveSpeed: {MoveSpeed}");
            return false;
        }

        if (RunSpeed <= 0)
        {
            Debug.LogError($"Player RunSpeed is invalid. RunSpeed: {RunSpeed}");
            return false;
        }

        if (JumpForce <= 0)
        {
            Debug.LogError($"Player JumpForce is invalid. JumpForce: {JumpForce}");
            return false;
        }

        if (RotateSpeed <= 0)
        {
            Debug.LogError($"Player RotateSpeed is invalid. RoateSpeed: {RotateSpeed}");
            return false;
        }

        return true;
    }
}
