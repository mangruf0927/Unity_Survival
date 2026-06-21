using UnityEngine;

public class Bed : MonoBehaviour
{
    [SerializeField] private int bonusDays = 1;

    private TimeSystem timeSystem;
    private bool isPlaced;

    public void OnPlaced(TimeSystem timeSystem)
    {
        if (isPlaced || timeSystem == null) return;
        this.timeSystem = timeSystem;

        timeSystem.AddDays(bonusDays);
        isPlaced = true;
    }

    private void OnDestroy()
    {
        if (isPlaced && timeSystem != null)
        {
            timeSystem.RemoveDays(bonusDays);
        }
    }
}
