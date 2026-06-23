using System;

[Serializable]
public class SaveData
{
    public TimeSaveData timeData;
}

[Serializable]
public class TimeSaveData
{
    public int dayCount;
    public int dayBonus;
    public float timeElapsed;
    public Phase curPhase;
}
