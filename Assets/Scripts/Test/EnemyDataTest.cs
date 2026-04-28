using UnityEngine;

public class EnemyDataTest : MonoBehaviour
{
    private string _name;
    private int _maxHP;
    private int _attackDamage;

    public void SetUp(EnemyDataTable data)
    {
        _name = data.Name;
        _maxHP = data.MaxHp;
        _attackDamage = data.AttackDamage;

        Debug.Log($"Enemy SetUp 완료: {_name}, HP: {_maxHP}, Damage: {_attackDamage}");
    }
}
