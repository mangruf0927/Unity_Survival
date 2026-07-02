using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour, ISubject
{
    [SerializeField] private ParticleSystem fire;

    [SerializeField] private List<float> decreaseTimeList = new() { 1f, 5f, 10f, 15f, 20f };
    [SerializeField] private int decreaseAmount;

    [SerializeField] private float levelUpDelay = 10f;
    [SerializeField] private float warningThreshold = 20f;

    private readonly List<IObserver> observerList = new();
    private const float MaxFuel = 100f;
    private const int MaxLevel = 5;

    private int currentLevel = 1;
    private float currentFuel = 0f;
    private float decreaseTimer;
    private bool isBurning;
    private bool isWarned;

    private bool isLevelingUp;
    private int pendingLevel;
    private float pendingCurrentFuel;
    private Coroutine levelUpDelayCoroutine;

    private Coroutine decreaseCoroutine;

    public int CurrentLevel => currentLevel;
    public float CurrentFuel => currentFuel;
    public float DecreaseTimer => decreaseTimer;
    public bool IsBurning => isBurning;
    public bool IsLevelingUp => isLevelingUp;

    public event Action<int> OnLevelUp;
    public event Action<bool> OnFireChanged;

    public event Action<CampFireNoticeType> OnNotice;

    private void Start()
    {
        OffFire();
    }

    public CampFireSaveData CreateSaveData()
    {
        int saveLevel = currentLevel;
        float saveFuel = currentFuel;
        if (isLevelingUp)
        {
            saveLevel = pendingLevel;
            saveFuel = pendingCurrentFuel;
        }

        return new CampFireSaveData
        {
            currentLevel = saveLevel,
            currentFuel = Mathf.Min(saveFuel, MaxFuel),
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

        if (levelUpDelayCoroutine != null)
        {
            StopCoroutine(levelUpDelayCoroutine);
            levelUpDelayCoroutine = null;
        }

        isLevelingUp = false;
        pendingLevel = currentLevel;
        pendingCurrentFuel = 0f;
        isWarned = false;

        currentLevel = Mathf.Clamp(data.currentLevel, 1, MaxLevel);
        currentFuel = Mathf.Clamp(data.currentFuel, 0f, MaxFuel);
        decreaseTimer = data.decreaseTimer;

        if (data.isBurning && currentFuel > 0)
        {
            OnFire();
            CheckLowFuelWarning();
            StartDecreaseFuel();
        }
        else
        {
            decreaseTimer = 0f;
            OffFire();
        }

        OnLevelUp?.Invoke(currentLevel);
        NotifyObservers();
    }

    private void AddFuel(float amount)
    {
        if (amount <= 0) return;

        if (isLevelingUp)
        {
            AddPendingFuel(amount);
            NotifyObservers();
            return;
        }

        currentFuel += amount;
        if (currentFuel > warningThreshold) isWarned = false;

        if (CanLevelUp())
        {
            LevelUp();
        }
        else if (currentLevel >= MaxLevel)
        {
            currentFuel = Mathf.Min(currentFuel, MaxFuel);
        }

        OnFire();

        if (!isLevelingUp)
        {
            StartDecreaseFuel();
        }

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

            float decreaseTime = GetDecreaseTime();

            if (decreaseTimer >= decreaseTime)
            {
                decreaseTimer -= decreaseTime;
                currentFuel -= decreaseAmount;

                if (currentFuel <= 0)
                {
                    currentFuel = 0f;
                    decreaseTimer = 0f;
                    isWarned = false;
                    OffFire();
                    OnNotice?.Invoke(CampFireNoticeType.EXTINGUISHED);
                    NotifyObservers();
                    decreaseCoroutine = null;
                    break;
                }

                CheckLowFuelWarning();
                NotifyObservers();
            }
            yield return null;
        }
        decreaseCoroutine = null;
    }

    private float GetDecreaseTime()
    {
        int index = currentLevel - 1;
        if (index < 0 || index >= decreaseTimeList.Count)
        {
            return 1f;
        }

        float decreaseTime = decreaseTimeList[index];
        if (decreaseTime <= 0f)
        {
            return 1f;
        }

        return decreaseTime;
    }

    private bool CanLevelUp()
    {
        if (currentLevel >= MaxLevel) return false;

        return currentFuel >= MaxFuel;
    }

    private void LevelUp()
    {
        currentLevel += 1;
        currentFuel = MaxFuel;
        isLevelingUp = true;
        pendingLevel = currentLevel;
        pendingCurrentFuel = Mathf.Ceil(MaxFuel * 0.29f);

        OnLevelUp?.Invoke(currentLevel);

        if (levelUpDelayCoroutine != null)
        {
            StopCoroutine(levelUpDelayCoroutine);
        }

        if (decreaseCoroutine != null)
        {
            StopCoroutine(decreaseCoroutine);
            decreaseCoroutine = null;
        }

        levelUpDelayCoroutine = StartCoroutine(LevelUpDelayRoutine());
    }

    private IEnumerator LevelUpDelayRoutine()
    {
        NotifyObservers();

        yield return new WaitForSeconds(levelUpDelay);

        currentLevel = pendingLevel;
        currentFuel = pendingCurrentFuel;
        pendingCurrentFuel = 0f;
        isLevelingUp = false;
        levelUpDelayCoroutine = null;

        currentFuel = Mathf.Min(currentFuel, MaxFuel);
        if (currentFuel > warningThreshold) isWarned = false;

        if (currentFuel > 0f)
        {
            OnFire();
            StartDecreaseFuel();
        }

        NotifyObservers();
    }

    private void AddPendingFuel(float amount)
    {
        pendingCurrentFuel += amount;

        while (pendingCurrentFuel >= MaxFuel && pendingLevel < MaxLevel)
        {
            pendingLevel += 1;
            pendingCurrentFuel = Mathf.Ceil(MaxFuel * 0.29f);
            OnLevelUp?.Invoke(pendingLevel);
        }

        pendingCurrentFuel = Mathf.Min(pendingCurrentFuel, MaxFuel);
    }

    private void CheckLowFuelWarning()
    {
        if (isWarned) return;
        if (currentLevel < 2) return;
        if (isLevelingUp) return;
        if (currentFuel > warningThreshold) return;

        isWarned = true;
        OnNotice?.Invoke(CampFireNoticeType.WARNING);
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

        AddFuel(item.Data.FuelData.GetBurnPower(currentLevel));
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
