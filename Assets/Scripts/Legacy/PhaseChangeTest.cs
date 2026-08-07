using UnityEngine;

public class PhaseChangeTest : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;

    private void OnEnable()
    {
        timeSystem.OnPhaseChanged += PhaseChanged;
    }

    private void OnDisable()
    {
        timeSystem.OnPhaseChanged -= PhaseChanged;
    }

    private void PhaseChanged(Phase phase, int day)
    {
        Debug.Log($"{day}일차 {phase}시작");
    }
}
