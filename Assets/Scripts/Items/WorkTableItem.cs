using System;
using UnityEngine;

[Serializable]
public class WorkTableItem
{
    public Sprite iconImage;

    public string itemName;
    public PlaceableItem itemPrefab;

    public int needWood;
    public int needIron;

    public int requiredLevel = 1;
    public bool unlocksNextLevel;
}
