using UnityEngine;
using UnityEngine.UI;

public class PlayerHungerBar : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private Image runIcon;

    private void Start()
    {
        playerStats.AddObserver(this);
        Notify();
    }

    private void OnDestroy()
    {
        playerStats.RemoveObserver(this);
    }

    public void Notify()
    {
        runIcon.enabled = playerStats.IsRun;

        hungerSlider.maxValue = playerStats.MaxHunger;
        hungerSlider.value = playerStats.CurrentHunger;
    }
}
