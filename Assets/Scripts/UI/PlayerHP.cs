using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private PlayerStat playerStat;

    private void Start()
    {
        playerStat.AddObserver(this);
        Notify();       
    }

    private void OnDisable()
    {
        playerStat.RemoveObserver(this);
    }

    public void Notify()
    {
        hpSlider.maxValue = playerStat.MaxHP;
        hpSlider.value = playerStat.CurrentHP;
        // Debug.Log(playerStat.MaxHP + " " + playerStat.CurrentHP);
    }
}
