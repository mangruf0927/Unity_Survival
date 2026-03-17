using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private PlayerStats playerStats;

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
        hpSlider.maxValue = playerStats.MaxHP;
        hpSlider.value = playerStats.CurrentHP;
        // Debug.Log(playerStat.MaxHP + " " + playerStat.CurrentHP);
    }
}
