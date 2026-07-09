using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour, IObserver
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI dayCountText;

    private void OnEnable()
    {
        timeSystem.AddObserver(this);
        Notify();
    }

    private void OnDisable()
    {
        timeSystem.RemoveObserver(this);
    }

    public void Notify()
    {
        dayCountText.text = $"Day {timeSystem.DayCount}";

        timerText.gameObject.SetActive(timeSystem.IsTimerUnlocked);

        if (!timeSystem.IsTimerUnlocked) return;

        timerText.text = $"{timeSystem.Minutes:00} : {timeSystem.Seconds:00}";
    }
}
