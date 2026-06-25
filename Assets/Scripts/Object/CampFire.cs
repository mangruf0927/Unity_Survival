using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// { 1, 100, 200, 750, 2300, 5000 }
public class CampFire : MonoBehaviour, ISubject
{
    [SerializeField] private ParticleSystem fire;

    [SerializeField] private int[] fuelLevelList;
    [SerializeField] private float intervalTime;
    [SerializeField] private int decreaseAmount;

    private readonly List<IObserver> observerList = new();
    private const int MaxLevel = 6;

    private int currentLevel = 0;
    private int currentFuel = 0;
    private bool isBurning;
    private Coroutine decreaseCoroutine;

    public int CurrentLevel => currentLevel;
    public int CurrentFuel => currentFuel;
    public bool IsBurning => isBurning;
    public int NeedFuel
    {
        get
        {
            if (currentLevel >= fuelLevelList.Length) return currentFuel;
            return fuelLevelList[currentLevel];
        }
    }

    public event Action<int> OnLevelUp;
    public event Action<bool> OnFireChanged;

    private void Start()
    {
        OffFire();
    }

    private void AddFuel(int amount)
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
            yield return new WaitForSeconds(intervalTime);

            currentFuel -= decreaseAmount;
            // Debug.Log(currentFuel);

            if (currentFuel <= 0)
            {
                currentFuel = 0;
                OffFire();
                NotifyObservers();
                break;
            }

            NotifyObservers();
        }
        decreaseCoroutine = null;
    }

    private bool CanLevelUp()
    {
        if (currentLevel >= MaxLevel) return false;
        if (currentLevel >= fuelLevelList.Length) return false;

        return currentFuel >= fuelLevelList[currentLevel];
    }

    private void LevelUp()
    {
        currentLevel += 1;

        if (currentLevel < fuelLevelList.Length)
        {
            currentFuel = Mathf.CeilToInt(fuelLevelList[currentLevel] * 0.29f);
        }
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

        AddFuel(item.Data.FuelData.BurnPower);
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
