using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public TimeSaveData timeData;
    public PlayerSaveData playerData;
    public InventorySaveData inventoryData;
}

[Serializable]
public class TimeSaveData
{
    public int dayCount;
    public int dayBonus;
    public float timeElapsed;
    public Phase curPhase;
}

[Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationY;

    public int currentHP;
    public float currentHunger;
    public float hungerTimer;
}

[Serializable]
public class InventorySaveData
{
    public List<int> itemIdList;
    public int equippedIndex;
    public SackSaveData sackData;
}

[Serializable]
public class SackSaveData
{
    public List<int> itemIdList;
}