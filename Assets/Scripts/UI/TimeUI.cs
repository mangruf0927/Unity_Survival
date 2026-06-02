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
        timerText.text = $"{timeSystem.Minutes:00} : {timeSystem.Seconds:00}";
        dayCountText.text = $"Day {timeSystem.DayCount}";
    }
}
