using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour, IDamageable, ISubject
{
    [SerializeField] private PlayerStateMachine playerStateMachine;

    private readonly List<IObserver> ObserverList = new();

    private int maxHp;
    private float moveSpeed;
    private float runSpeed;
    private float jumpForce;
    private float rotateSpeed;
    private float interactDistance;
    private float maxHunger;
    private float decreaseInterval;
    private float recoveryInterval;

    private bool isSetUp;
    private float hungerTimer;
    private bool isRun;
    private float recoveryTimer;

    public int MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float RotateSpeed => rotateSpeed;
    public float MaxHunger => maxHunger;
    public float InteractDistance => interactDistance;
    public float RecoveryInterval => recoveryInterval;

    public bool IsRun => isRun;

    public int CurrentHp { get; private set; }
    public float CurrentHunger { get; private set; }

    private Transform PlayerRoot => transform.root;

    private void Start()
    {
        PlayerData data = DataManager.Instance.PlayerTable.Get(1001);
        SetUp(data);
    }

    private void Update()
    {
        if (!isSetUp) return;

        UpdateHunger();
        UpdateHpRecovery();
    }

    public void SetUp(PlayerData data)
    {
        maxHp = data.MaxHp;
        moveSpeed = data.MoveSpeed;
        runSpeed = data.RunSpeed;
        jumpForce = data.JumpForce;
        rotateSpeed = data.RotateSpeed;
        interactDistance = data.InteractDistance;
        maxHunger = data.MaxHunger;
        decreaseInterval = data.DecreaseInterval;
        recoveryInterval = data.RecoveryInterval;

        CurrentHp = maxHp;
        CurrentHunger = maxHunger;

        isSetUp = true;
        NotifyObservers();
    }

    public void SetRun(bool isTrue)
    {
        if (isRun == isTrue) return;

        isRun = isTrue;
        NotifyObservers();
    }

    private void UpdateHunger()
    {
        float multiplier = isRun ? 1.3f : 1f;
        hungerTimer += Time.deltaTime * multiplier;

        while (hungerTimer >= decreaseInterval)
        {
            hungerTimer -= decreaseInterval;
            if (CurrentHunger > 0f)
            {
                CurrentHunger = Mathf.Max(CurrentHunger - 1, 0);
                NotifyObservers();
            }
            else
            {
                Debug.Log("Player is starving.");
                TakeDamage(10);
            }
        }
    }

    public void EatFood(int hunger, int hp)
    {
        if (hunger == 0 && hp == 0) return;

        CurrentHunger = Mathf.Clamp(CurrentHunger + hunger, 0, MaxHunger);
        CurrentHp = Mathf.Clamp(CurrentHp + hp, 0, MaxHp);

        NotifyObservers();
    }

    private void UpdateHpRecovery()
    {
        if (CurrentHp <= 0 || CurrentHp >= MaxHp || CurrentHunger <= 0)
        {
            recoveryTimer = 0f;
            return;
        }

        recoveryTimer += Time.deltaTime;

        if (recoveryTimer < recoveryInterval) return;
        recoveryTimer -= recoveryInterval;

        CurrentHp = Mathf.Min(CurrentHp + 1, maxHp);
        NotifyObservers();
    }

    public void RecoverHp(int hp)
    {
        if (hp <= 0) return;

        CurrentHp = Mathf.Clamp(CurrentHp + hp, 0, maxHp);
        NotifyObservers();
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHp <= 0) return;

        recoveryTimer = 0f;
        CurrentHp = Mathf.Max(CurrentHp - dmg, 0);
        NotifyObservers();

        if (CurrentHp <= 0) playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }

    public PlayerSaveData CreateSaveData()
    {
        Transform playerRoot = PlayerRoot;
        Vector3 position = playerRoot.position;

        return new PlayerSaveData
        {
            positionX = position.x,
            positionY = position.y,
            positionZ = position.z,
            rotationY = playerRoot.eulerAngles.y,
            currentHP = CurrentHp,
            currentHunger = CurrentHunger,
            hungerTimer = hungerTimer
        };
    }

    public void LoadSaveData(PlayerSaveData data)
    {
        if (data == null) return;

        Vector3 position = new(data.positionX, data.positionY, data.positionZ);
        PlayerRoot.SetPositionAndRotation(position, Quaternion.Euler(0f, data.rotationY, 0f));

        CurrentHp = Mathf.Clamp(data.currentHP, 0, MaxHp);
        CurrentHunger = Mathf.Clamp(data.currentHunger, 0f, MaxHunger);
        hungerTimer = Mathf.Max(0f, data.hungerTimer);
        recoveryTimer = 0f;

        NotifyObservers();
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
