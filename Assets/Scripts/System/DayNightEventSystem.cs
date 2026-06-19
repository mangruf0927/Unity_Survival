using UnityEngine;

public class DayNightEventSystem : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private CultistSpawner cultistSpawner;

    [SerializeField] private int raidInterval = 4;
    [SerializeField] private int meleeCultistCount = 2;

    private void Start()
    {
        timeSystem.OnPhaseChanged += PhaseChanged;
    }

    private void OnDestroy()
    {
        timeSystem.OnPhaseChanged -= PhaseChanged;
    }

    private void PhaseChanged(Phase phase, int day)
    {
        if (phase != Phase.NIGHT) return;
        if (day % raidInterval != 0) return;

        cultistSpawner.Spawn(PoolTypeEnums.MELEE_CULTIST, meleeCultistCount);
        Debug.Log($"{day}일차 신도 습격 시작");
    }
}
