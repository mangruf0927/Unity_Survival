using UnityEngine;

public class SunDial : MonoBehaviour
{
    private TimeSystem timeSystem;
    private bool isPlaced;

    public void OnPlaced(TimeSystem timeSystem)
    {
        if (isPlaced || timeSystem == null) return;
        this.timeSystem = timeSystem;

        timeSystem.SetTimerUnlocked(true);
        isPlaced = true;
    }

    private void OnDestroy()
    {
        if (isPlaced && timeSystem != null)
        {
            timeSystem.SetTimerUnlocked(false);
        }
    }
}
