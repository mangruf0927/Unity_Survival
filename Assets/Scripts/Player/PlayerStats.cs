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
    private float maxHunger;
    private int decreaseInterval;

    private bool isSetUp;
    private float timer;

    public int MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float RotateSpeed => rotateSpeed;
    public float MaxHunger => maxHunger;

    public int CurrentHp { get; private set; }
    public float CurrentHunger { get; private set; }

    private void Start()
    {
        PlayerData data = DataManager.Instance.PlayerTable.Get(1001);
        SetUp(data);
    }

    private void Update()
    {
        if (isSetUp) UpdateHunger();
    }

    public void SetUp(PlayerData data)
    {
        maxHp = data.MaxHp;
        moveSpeed = data.MoveSpeed;
        runSpeed = data.RunSpeed;
        jumpForce = data.JumpForce;
        rotateSpeed = data.RotateSpeed;
        maxHunger = data.MaxHunger;
        decreaseInterval = data.DecreaseInterval;

        CurrentHp = maxHp;
        CurrentHunger = maxHunger;

        isSetUp = true;
        NotifyObservers();
    }

    private void UpdateHunger()
    {
        if (CurrentHunger <= 0f) return;

        timer += Time.deltaTime;

        while (timer >= decreaseInterval && CurrentHunger > 0f)
        {
            timer -= decreaseInterval;
            CurrentHunger = Mathf.Max(CurrentHunger - 1, 0);
            NotifyObservers();
        }

        if (CurrentHunger <= 0f) Debug.Log("Player is starving.");
    }

    public void EatFood(int hunger, int hp)
    {
        if (hunger <= 0 && hp <= 0) return;

        if (hunger > 0) CurrentHunger = Mathf.Min(CurrentHunger + hunger, MaxHunger);
        if (hp > 0) CurrentHp = Mathf.Min(CurrentHp + hp, MaxHp);

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
