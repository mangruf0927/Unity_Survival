using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public TimeSaveData timeData;
    public PlayerSaveData playerData;
    public InventorySaveData inventoryData;
    public CampFireSaveData campFireData;
    public WorkTableSaveData workTableSaveData;
    public List<CultistSaveData> cultistSaveDataList;
    public List<ItemSaveData> itemSaveDataList;
    public List<EquippableSaveData> equippableSaveDataList;
    public WorldSaveData worldSaveData;
}

[Serializable]
public class TimeSaveData
{
    public int dayCount;
    public int dayBonus;
    public int cycleCount;

    public int raidCount;
    public int lastRaidCycle;

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
    public float currentFuel;
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

[Serializable]
public class WorkTableSaveData
{
    public int iron;
    public int wood;
    public int currentLevel;
    public List<PurchaseSaveData> purchaseDataList;
}

[Serializable]
public class PurchaseSaveData
{
    public int itemIndex;
    public int purchaseCount;
}

[Serializable]
public class CultistSaveData
{
    public PoolTypeEnums cultistType;
    public CultistWeaponType weaponType;

    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationY;

    public int currentHp;
}

[Serializable]
public class ItemSaveData
{
    public int itemId;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;
}

[Serializable]
public class EquippableSaveData
{
    public int itemId;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;
}

[Serializable]
public class WorldSaveData
{
    public long nextInstanceId;
    public List<ObjectSaveData> objectSaveDataList;
}

[Serializable]
public class ObjectSaveData
{
    public long instanceId;
    public int itemId;
    public ObjectType objectType;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public bool isActive;

    public TreeSaveData treeSaveData;
    public ChestSaveData chestSaveData;
}

[Serializable]
public class TreeSaveData
{
    public int currentHp;
}

[Serializable]
public class ChestSaveData
{
    public bool isOpened;
}
