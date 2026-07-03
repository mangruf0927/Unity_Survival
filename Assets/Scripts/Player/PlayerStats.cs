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
    private int decreaseInterval;

    private bool isSetUp;
    private float timer;

    public int MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float RotateSpeed => rotateSpeed;
    public float MaxHunger => maxHunger;
    public float InteractDistance => interactDistance;

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
        if (isSetUp) UpdateHunger();
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

        CurrentHp = maxHp;
        CurrentHunger = maxHunger;

        isSetUp = true;
        NotifyObservers();
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
            hungerTimer = timer
        };
    }

    public void LoadSaveData(PlayerSaveData data)
    {
        if (data == null) return;

        Vector3 position = new(data.positionX, data.positionY, data.positionZ);
        PlayerRoot.SetPositionAndRotation(position, Quaternion.Euler(0f, data.rotationY, 0f));

        CurrentHp = Mathf.Clamp(data.currentHP, 0, MaxHp);
        CurrentHunger = Mathf.Clamp(data.currentHunger, 0f, MaxHunger);
        timer = Mathf.Max(0f, data.hungerTimer);

        NotifyObservers();
    }

    private void UpdateHunger()
    {
        timer += Time.deltaTime;

        while (timer >= decreaseInterval)
        {
            timer -= decreaseInterval;
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

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHp <= 0) return;

        CurrentHp = Mathf.Max(CurrentHp - dmg, 0);
        // Debug.Log($"Player hit. Damage: {dmg}, CurrentHp: {CurrentHp}");
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
