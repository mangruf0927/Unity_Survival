using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour, ISubject
{
    private readonly float dayTime = 180f;
    private readonly float nightTime = 90f;

    private float timeElapsed;
    private Phase curPhase;

    private readonly List<IObserver> ObserverList = new();

    private int dayCount = 1;
    private int minutes;
    private int seconds;

    public int DayCount => dayCount;
    public int Minutes => minutes;
    public int Seconds => seconds;

    void Start()
    {
        SetPhase(Phase.DAY);
    }

    void Update()
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
                dayCount += 1;
                SetPhase(Phase.DAY);
            }
            return;
        }
        UpdateTimer();
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
