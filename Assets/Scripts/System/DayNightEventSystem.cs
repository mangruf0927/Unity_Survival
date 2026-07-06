using UnityEngine;

public class DayNightEventSystem : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private CultistSpawner cultistSpawner;

    [SerializeField] private int raidInterval = 4;

    private int raidCount;
    private int lastRaidCycle;

    private void OnEnable()
    {
        if (timeSystem != null)
            timeSystem.OnPhaseChanged += PhaseChanged;
    }

    private void OnDisable()
    {
        if (timeSystem != null)
            timeSystem.OnPhaseChanged -= PhaseChanged;
    }

    public void CreateSaveData(TimeSaveData data)
    {
        data.raidCount = raidCount;
        data.lastRaidCycle = lastRaidCycle;
    }

    public void LoadSaveData(TimeSaveData data)
    {
        if (data == null)
        {
            raidCount = 0;
            lastRaidCycle = 0;
            return;
        }

        raidCount = data.raidCount;
        lastRaidCycle = data.lastRaidCycle;
    }

    private void PhaseChanged(Phase phase, int day)
    {
        if (phase != Phase.NIGHT) return;

        int elapsedCycle = timeSystem.CycleCount;

        if (elapsedCycle <= 0 || elapsedCycle % raidInterval != 0) return;
        if (lastRaidCycle == elapsedCycle) return;

        StartRaid(elapsedCycle);
    }

    private void StartRaid(int elapsedCycle)
    {
        lastRaidCycle = elapsedCycle;
        raidCount++;

        SpawnRaid();

        Debug.Log($"{timeSystem.DayCount}일차 신도 습격 시작 / 실제 경과 사이클: {elapsedCycle} / 습격 횟수: {raidCount}");
    }

    private void SpawnRaid()
    {
        if (timeSystem.DayCount >= 50)
        {
            cultistSpawner.Spawn(PoolTypeEnums.MELEE_CULTIST, CultistWeaponType.MORNINGSTAR, 4);
            cultistSpawner.Spawn(PoolTypeEnums.RANGED_CULTIST, CultistWeaponType.CROSSBOW, 2);
            return;
        }

        if (raidCount == 1)
        {
            cultistSpawner.Spawn(PoolTypeEnums.MELEE_CULTIST, CultistWeaponType.AXE, 2);
            return;
        }

        cultistSpawner.Spawn(PoolTypeEnums.MELEE_CULTIST, CultistWeaponType.SPEAR, 3);
        cultistSpawner.Spawn(PoolTypeEnums.RANGED_CULTIST, CultistWeaponType.CROSSBOW, 1);
    }
}
