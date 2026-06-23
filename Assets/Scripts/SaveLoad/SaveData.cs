using System;

[Serializable]
public class SaveData
{
    public TimeSaveData timeData;
    public PlayerSaveData playerData;
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
