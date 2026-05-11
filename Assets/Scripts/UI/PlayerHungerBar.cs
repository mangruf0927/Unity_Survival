using UnityEngine;
using UnityEngine.UI;

public class PlayerHungerBar : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private PlayerStats playerStats;

    private void Start()
    {
        playerStats.AddObserver(this);
    }

    private void OnDestroy()
    {
        playerStats.RemoveObserver(this);
    }

    public void Notify()
    {
        hungerSlider.maxValue = playerStats.MaxHunger;
        hungerSlider.value = playerStats.CurrentHunger;
    }
}
