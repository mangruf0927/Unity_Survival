using System.Collections;
using UnityEngine;

public class DataLoadTest : MonoBehaviour
{
    [SerializeField] private EnemyDataTest enemyDataTest;

    private void Start()
    {
        EnemyData slime = DataManager.Instance.EnemyTable.Get(1001);
        Debug.Log($"Id: {slime.Id}, Name: {slime.Name}, MaxHp: {slime.MaxHp}, AttackDamage: {slime.AttackDamage}");

        if (enemyDataTest != null) enemyDataTest.SetUp(slime);
    }
}
