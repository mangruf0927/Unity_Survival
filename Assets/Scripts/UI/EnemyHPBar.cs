using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour, IObserver
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private EnemyStats enemyStats;

    private void Start()
    {
        enemyStats.AddObserver(this);
        Notify();       
    }

    private void OnDestroy()
    {
        enemyStats.RemoveObserver(this);
    }

    public void Notify()
    {
        hpSlider.maxValue = enemyStats.MaxHP;
        hpSlider.value = enemyStats.CurrentHP;
    }
}
