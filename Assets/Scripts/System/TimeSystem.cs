using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour, ISubject
{
    [SerializeField] private float dayTime = 180f;
    [SerializeField] private float nightTime = 90f;

    private readonly List<IObserver> ObserverList = new();

    private float timeElapsed;
    private Phase curPhase;

    private int cycleCount = 1;
    private int dayCount = 1;
    private int dayBonus;
    private int minutes;
    private int seconds;
    private bool isTimerUnlocked;

    public int CycleCount => cycleCount;
    public int DayCount => dayCount;
    public int Minutes => minutes;
    public int Seconds => seconds;
    public bool IsTimerUnlocked => isTimerUnlocked;
    public Phase CurPhase => curPhase;

    public event Action<Phase, int> OnPhaseChanged;

    private void Start()
    {
        SetPhase(Phase.DAY);
    }

    private void Update()
    {
        timeElapsed -= Time.deltaTime;

        if (timeElapsed <= 0f)
        {
            if (curPhase == Phase.DAY)
            {
                SetPhase(Phase.NIGHT);
            }
            else
            {
                cycleCount++;
                dayCount += 1 + dayBonus;
                SetPhase(Phase.DAY);
            }
            return;
        }
        UpdateTimer();
    }

    public TimeSaveData CreateSaveData()
    {
        return new TimeSaveData
        {
            dayCount = dayCount,
            dayBonus = dayBonus,
            cycleCount = cycleCount,
            timeElapsed = timeElapsed,
            curPhase = curPhase
        };
    }

    public void LoadSaveData(TimeSaveData data)
    {
        dayCount = data.dayCount;
        dayBonus = data.dayBonus;
        cycleCount = data.cycleCount;
        timeElapsed = data.timeElapsed;
        curPhase = data.curPhase;

        UpdateTimer();
        OnPhaseChanged?.Invoke(curPhase, dayCount);
    }

    private void UpdateTimer()
    {
        minutes = Mathf.FloorToInt(timeElapsed / 60F);
        seconds = Mathf.FloorToInt(timeElapsed % 60F);
        NotifyObservers();
    }

    private void SetPhase(Phase phase)
    {
        curPhase = phase;
        timeElapsed = (phase == Phase.DAY) ? dayTime : nightTime;

        UpdateTimer();
        OnPhaseChanged?.Invoke(curPhase, dayCount);
    }

    public void AddDays(int days)
    {
        dayBonus += days;
    }

    public void RemoveDays(int days)
    {
        dayBonus = Mathf.Max(0, dayBonus - days);
    }

    public void SetTimerUnlocked(bool isUnlocked)
    {
        isTimerUnlocked = isUnlocked;
        NotifyObservers();
    }

    // >>
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
