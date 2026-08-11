using UnityEngine;

public class CultistStats : EnemyStatsBase
{
    private float scanRange;
    private float maxRaidCenterDistance;
    private float returnDistance;
    private float returnSearchRange;
    private float alertDuration;
    private PoolTypeEnums cultistType;

    public int CultistId => Id;
    public float ScanRange => scanRange;
    public float MaxRaidCenterDistance => maxRaidCenterDistance;
    public float ReturnDistance => returnDistance;
    public float ReturnSearchRange => returnSearchRange;
    public float AlertDuration => alertDuration;
    public PoolTypeEnums CultistType => cultistType;

    public void SetUp(CultistData data)
    {
        if (data == null)
            return;

        scanRange = data.ScanRange;
        maxRaidCenterDistance = data.MaxRaidCenterDistance;
        returnDistance = data.ReturnDistance;
        returnSearchRange = data.ReturnSearchRange;
        alertDuration = data.AlertDuration;
        cultistType = data.CultistType;

        InitializeHp(data.MaxHp);
    }
}