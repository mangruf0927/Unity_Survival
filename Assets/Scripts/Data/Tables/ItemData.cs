using System.Collections.Generic;
using UnityEngine;

public class ItemData : IGameData, IValidatable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }

    public ItemType ItemType { get; set; }
    public ItemProperty ItemProperty { get; set; }

    public FuelData FuelData { get; set; }
    public FoodData FoodData { get; set; }
    public MaterialData MaterialData { get; set; }
    public AmmoData AmmoData { get; set; }

    public bool HasProperty(ItemProperty property)
    {
        return (ItemProperty & property) != 0;
    }

    public bool Validate()
    {
        if (Id <= 0)
        {
            Debug.LogError($"Item Id is invalid. Id: {Id}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            Debug.LogError($"Item Name is empty. Id: {Id}");
            return false;
        }

        if (Value < 0)
        {
            Debug.LogError($"Item Value is invalid. Id: {Id}, Value: {Value}");
            return false;
        }

        if (ItemType == ItemType.FUEL)
        {
            if (FuelData == null || FuelData.BurnPowerList == null || FuelData.BurnPowerList.Count == 0)
            {
                Debug.LogError($"FuelData is missing. Id: {Id}, Name: {Name}");
                return false;
            }

            foreach (float burnPower in FuelData.BurnPowerList)
            {
                if (burnPower < 0f)
                {
                    Debug.LogError($"BurnPower is invalid. Id: {Id}, Name: {Name}, BurnPower: {burnPower}");
                    return false;
                }
            }
        }

        return true;
    }
}

public class FuelData
{
    public List<float> BurnPowerList { get; set; }

    public float GetBurnPower(int campFireLevel)
    {
        if (BurnPowerList == null || BurnPowerList.Count == 0) return 0f;

        int index = Mathf.Clamp(campFireLevel - 1, 0, BurnPowerList.Count - 1);
        return BurnPowerList[index];
    }
}

public class FoodData
{
    public int HungerAmount { get; set; }
    public int HpAmount { get; set; }
    public bool NeedCook { get; set; }
}

public class MaterialData
{
    public int GrindCount { get; set; }
    public MaterialType Type { get; set; }
}

public class AmmoData
{
    public AmmoType AmmoType { get; set; }
    public int Amount { get; set; }
}
