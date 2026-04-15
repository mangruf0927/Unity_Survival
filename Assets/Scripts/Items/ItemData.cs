using System;
using UnityEngine;

[Serializable]
public struct FuelData
{
    [SerializeField] private int burnPower;
    public int BurnPower => burnPower;
}

[Serializable]
public struct FoodData
{
    [SerializeField] private int hungerAmount;
    [SerializeField] private int hpAmount;
    [SerializeField] private bool needCook;

    public int HungerAmount => hungerAmount;
    public int HpAmount => hpAmount;
    public bool NeedCook => needCook;
}

[Serializable]
public struct MaterialData
{
    [SerializeField] private int grindCount;
    [SerializeField] private MaterialType materialType;

    public int GrindCount => grindCount;
    public MaterialType Type => materialType;
}

[Serializable]
public struct AmmoData
{
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private int amount;

    public AmmoType AmmoType => ammoType;
    public int Amount => amount;
}

[Serializable]
public class ItemData
{
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private ItemProperty itemProperty;

    [SerializeField] private FuelData fuelData;
    [SerializeField] private FoodData foodData;
    [SerializeField] private MaterialData materialData;
    [SerializeField] private AmmoData ammoData;

    public string ItemName => itemName;
    public ItemType ItemType => itemType;
    public ItemProperty ItemProperty => itemProperty;

    public FuelData FuelData => fuelData;
    public FoodData FoodData => foodData;
    public MaterialData MaterialData => materialData;
    public AmmoData AmmoData => ammoData;

    public bool HasProperty(ItemProperty property)
    {
        return (itemProperty & property) != 0;
    }
}
