using System.Collections.Generic;
using UnityEngine;

public class WorkTableInventory : MonoBehaviour, ISubject
{
    [SerializeField] private WorkTableItem[] itemList;

    private int wood = 0;
    private int iron = 0;

    public int Wood => wood;
    public int Iron => iron;

    private readonly List<IObserver> ObserverList = new();

    public void AddMaterial(MaterialType type, int amount)
    {
        if (type == MaterialType.WOOD)
        {
            wood += amount;
        }
        else if (type == MaterialType.IRON)
        {
            iron += amount;
        }

        NotifyObservers();
    }

    public bool CanUse(int needWood, int needIron)
    {
        return wood >= needWood && iron >= needIron;
    }

    public bool UseMaterial(int needWood, int needIron)
    {
        if (!CanUse(needWood, needIron)) return false;

        wood -= needWood;
        iron -= needIron;

        NotifyObservers();
        return true;
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
