using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBase : MonoBehaviour, IDamageable, ISubject
{
    private readonly List<IObserver> ObserverList = new();
    public abstract int MaxHP { get; }
    public int CurrentHP { get; private set; }

    protected virtual void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || CurrentHP <= 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        NotifyObservers();

        if (CurrentHP <= 0)
        {
            Die();
        }
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

    protected abstract void Die();
}
