using System.Collections;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    [SerializeField] private int maxLevel = 6;
    [SerializeField] private ParticleSystem fire;

    [SerializeField] private int[] fuelLevelList;
    [SerializeField] private float intervalTime;
    [SerializeField] private int decreaseAmount;

    private int currentLevel = 0;
    private int currentFuel = 0;

    public int CurrentLevel => currentLevel;
    public int CurrentFuel => currentFuel;
    private Coroutine decreaseCoroutine;

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
            Debug.Log(currentFuel);

            if (currentFuel <= 0)
            {
                currentFuel = 0;
                OffFire();
                break;
            }
        }
        decreaseCoroutine = null;
    }

    private bool CanLevelUp()
    {
        if (currentLevel >= maxLevel) return false;
        if (currentLevel >= fuelLevelList.Length) return false;
        if (currentFuel < fuelLevelList[currentLevel]) return false;

        return true;
    }

    private void LevelUp()
    {
        currentLevel += 1;
    }

    private void OnFire()
    {
        if (!fire.isPlaying) fire.Play();
    }

    private void OffFire()
    {
        fire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item == null) return;
        if (item.Data.ItemType != ItemType.FUEL) return;

        AddFuel(item.Data.FuelData.BurnPower);
        Destroy(other.gameObject);
    }
}