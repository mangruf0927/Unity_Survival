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

        return true;
    }
}

public class FuelData
{
    public int BurnPower { get; set; }
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