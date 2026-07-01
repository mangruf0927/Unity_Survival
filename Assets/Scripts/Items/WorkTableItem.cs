using System;
using UnityEngine;

[Serializable]
public class WorkTableItem
{
    public Sprite iconImage;

    public string itemName;
    [TextArea] public string description;
    public PlaceableItem itemPrefab;

    public int needIron;
    public int needWood;

    public int requiredLevel = 1;
    public bool unlocksNextLevel;

    public int purchaseLimit = 1;
}
