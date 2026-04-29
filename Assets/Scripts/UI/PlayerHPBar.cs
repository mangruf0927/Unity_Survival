using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour, IObserver
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
        hpSlider.maxValue = playerStats.MaxHp;
        hpSlider.value = playerStats.CurrentHp;
    }
}
