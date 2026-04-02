using UnityEngine;

public class CampFire : MonoBehaviour
{
    [SerializeField] private int maxLevel = 6;
    [SerializeField] private int[] fuelLevelList = { 1, 6, 20, 40, 80, 140 };

    private int currentLevel = 0;
    private int currentFuel = 0;

    public int CurrentLevel => currentLevel;
    public int CurrentFuel => currentFuel;
    public bool IsLit => currentFuel > 0;

    private void AddFuel(int amount)
    {
        if (amount <= 0) return;
        currentFuel += amount;
        if (currentFuel >= fuelLevelList[currentLevel]) LevelUp();
    }

    private void LevelUp()
    {
        if (currentLevel >= maxLevel) return;
        currentLevel += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item == null) return;
        if (item.ItemType != ItemType.FUEL) return;

        AddFuel(item.Value);
        Destroy(other.gameObject);
    }
}
