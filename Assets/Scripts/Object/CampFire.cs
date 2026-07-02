using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour, ISubject
{
    [SerializeField] private ParticleSystem fire;

    [SerializeField] private float intervalTime;
    [SerializeField] private int decreaseAmount;

    private readonly List<IObserver> observerList = new();
    private const float MaxFuel = 100f;
    private const int MaxLevel = 5;

    private int currentLevel = 0;
    private float currentFuel = 0f;
    private bool isBurning;
    private float decreaseTimer;
    private Coroutine decreaseCoroutine;

    public int CurrentLevel => currentLevel;
    public float CurrentFuel => currentFuel;
    public bool IsBurning => isBurning;
    public float DecreaseTimer => decreaseTimer;

    public event Action<int> OnLevelUp;
    public event Action<bool> OnFireChanged;

    private void Start()
    {
        OffFire();
    }

    public CampFireSaveData CreateSaveData()
    {
        return new CampFireSaveData
        {
            currentLevel = currentLevel,
            currentFuel = currentFuel,
            isBurning = isBurning,
            decreaseTimer = decreaseTimer
        };
    }

    public void LoadSaveData(CampFireSaveData data)
    {
        if (data == null) return;

        if (decreaseCoroutine != null)
        {
            StopCoroutine(decreaseCoroutine);
            decreaseCoroutine = null;
        }

        currentLevel = data.currentLevel;
        currentFuel = data.currentFuel;
        decreaseTimer = data.decreaseTimer;

        if (data.isBurning && currentFuel > 0)
        {
            OnFire();
            StartDecreaseFuel();
        }
        else
        {
            decreaseTimer = 0f;
            OffFire();
        }

        OnLevelUp?.Invoke(currentLevel);
        OnFireChanged?.Invoke(isBurning);
        NotifyObservers();
    }

    private void AddFuel(float amount)
    {
        if (amount <= 0) return;

        currentFuel += amount;
        if (CanLevelUp()) LevelUp();

        OnFire();
        StartDecreaseFuel();

        NotifyObservers();
    }

    private void StartDecreaseFuel()
    {
        if (decreaseCoroutine != null) return;

        decreaseCoroutine = StartCoroutine(DecreaseFuel());
    }

    private IEnumerator DecreaseFuel()
    {
        while (currentFuel > 0)
        {
            decreaseTimer += Time.deltaTime;

            if (decreaseTimer >= intervalTime)
            {
                decreaseTimer -= intervalTime;
                currentFuel -= decreaseAmount;

                if (currentFuel <= 0)
                {
                    currentFuel = 0f;
                    decreaseTimer = 0f;
                    OffFire();
                    NotifyObservers();
                    decreaseCoroutine = null;
                    break;
                }
                NotifyObservers();
            }
            yield return null;
        }
        decreaseCoroutine = null;
    }

    private bool CanLevelUp()
    {
        if (currentLevel >= MaxLevel) return false;

        return currentFuel >= MaxFuel;
    }

    private void LevelUp()
    {
        currentLevel += 1;
        currentFuel = Mathf.Ceil(MaxFuel * 0.29f);
        OnLevelUp?.Invoke(currentLevel);
    }

    private void OnFire()
    {
        if (isBurning) return;

        isBurning = true;
        if (fire != null && !fire.isPlaying) fire.Play();
        OnFireChanged?.Invoke(true);
    }

    private void OffFire()
    {
        bool wasBurning = isBurning;
        isBurning = false;

        if (fire != null)
        {
            fire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (wasBurning)
        {
            OnFireChanged?.Invoke(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponentInParent<Item>();
        if (item == null) return;
        if (item.Data == null || item.Data.FuelData == null) return;
        if (item.Data.ItemType != ItemType.FUEL) return;

        int fuelLevel = Mathf.Max(currentLevel, 1);
        AddFuel(item.Data.FuelData.GetBurnPower(fuelLevel));
        Destroy(item.gameObject);
    }

    public void AddObserver(IObserver observer)
    {
        observerList.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observerList.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in observerList)
        {
            observer.Notify();
        }
    }
}
