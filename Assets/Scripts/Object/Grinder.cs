using System.Collections.Generic;
using UnityEngine;

public class Grinder : MonoBehaviour, ISubject
{
    private int wood = 0;
    private int iron = 0;

    public int Wood => wood;
    public int Iron => iron;

    private readonly List<IObserver> ObserverList = new();


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Item")) return;

        Item item = other.GetComponent<Item>();
        if (item == null) return;

        ItemData data = item.Data;
        if (data.ItemType == ItemType.MATERIAL || data.HasProperty(ItemProperty.MATERIAL))
        {
            int count = data.MaterialData.GrindCount;

            if (data.MaterialData.Type == MaterialType.WOOD) wood += count;
            else if (data.MaterialData.Type == MaterialType.IRON) iron += count;
            else return;

            NotifyObservers();
            Destroy(item.gameObject);       // pool 쓰는 경우 나중에 ReturnToPool로 변경
        }
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
