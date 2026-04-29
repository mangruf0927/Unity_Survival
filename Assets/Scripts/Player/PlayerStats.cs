using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerStats : MonoBehaviour, IDamageable, ISubject
{
    [SerializeField] private PlayerStateMachine playerStateMachine;

    private readonly List<IObserver> ObserverList = new();

    private int maxHp;
    private float moveSpeed;
    private float runSpeed;
    private float jumpForce;
    private float rotateSpeed;

    public int MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float RotateSpeed => rotateSpeed;

    public int CurrentHp { get; private set; }

    private IEnumerator Start()
    {
        if (!DataManager.Instance.IsLoaded) yield return DataManager.Instance.LoadAll();

        PlayerDataTable data = DataManager.Instance.PlayerTable.Get(1001);
        SetUp(data);
    }

    public void SetUp(PlayerDataTable data)
    {
        maxHp = data.MaxHp;
        moveSpeed = data.MoveSpeed;
        runSpeed = data.RunSpeed;
        jumpForce = data.JumpForce;
        rotateSpeed = data.RotateSpeed;

        CurrentHp = maxHp;
        NotifyObservers();
    }


    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHp <= 0) return;

        CurrentHp = Mathf.Max(CurrentHp - dmg, 0);
        NotifyObservers();

        if (CurrentHp <= 0) playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }

    public void AddObserver(IObserver observer)
    {
        ObserverList.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        if (observer == null) return;
        ObserverList.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in ObserverList)
        {
            observer.Notify();
        }
    }
}
