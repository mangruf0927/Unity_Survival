using TMPro;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI dayCountText;

    private readonly float day = 180f;
    private readonly float night = 90f;
    private int dayCount = 1;

    private float timeElapsed;
    private Phase curPhase;

    void Start()
    {
        SetPhase(Phase.DAY);
        UpdateTimer();
    }

    void Update()
    {
        timeElapsed -= Time.deltaTime;

        if (timeElapsed <= 0f)
        {
            timeElapsed = 0f;
            UpdateTimer();

            if (curPhase == Phase.DAY) SetPhase(Phase.NIGHT);
            else
            {
                dayCount += 1;
                SetPhase(Phase.DAY);
            }
        }
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60F);
        int seconds = Mathf.FloorToInt(timeElapsed % 60F);
        timerText.text = $"{minutes:00} : {seconds:00}";
        dayCountText.text = $"Day {dayCount}";
    }

    private void SetPhase(Phase phase)
    {
        curPhase = phase;
        timeElapsed = (phase == Phase.DAY) ? day : night;
    }
}
