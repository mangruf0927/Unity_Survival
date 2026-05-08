using UnityEngine;
using UnityEngine.UI;

public class PlayerHungerBar : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hpSlider;
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
        hpSlider.maxValue = playerStats.MaxHunger;
        hpSlider.value = playerStats.CurrentHunger;
    }
}
