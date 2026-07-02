using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public TimeSaveData timeData;
    public PlayerSaveData playerData;
    public InventorySaveData inventoryData;
    public CampFireSaveData campFireData;
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
public class CampFireSaveData
{
    public int currentLevel;
    public int currentFuel;
    public bool isBurning;
    public float decreaseTimer;
}

[Serializable]
public class InventorySaveData
{
    public List<int> itemIdList;
    public int equippedIndex;
    public SackSaveData sackData;
    public List<AmmoSaveData> ammoDataList;
    public List<RangedWeaponSaveData> rangedWeaponDataList;
}

[Serializable]
public class SackSaveData
{
    public List<int> itemIdList;
}

[Serializable]
public class AmmoSaveData
{
    public AmmoType ammoType;
    public int count;
}

[Serializable]
public class RangedWeaponSaveData
{
    public int itemId;
    public int currentAmmo;
}
