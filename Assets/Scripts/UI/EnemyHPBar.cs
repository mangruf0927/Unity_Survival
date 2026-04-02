using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private EnemyStats enemyStats;

    public void SetHPBar(EnemyStats stats)
    {
        enemyStats = stats;
        UpdateHPBar();
    }

    public void UpdateHPBar()
    {
        hpSlider.maxValue = enemyStats.MaxHP;
        hpSlider.value = enemyStats.CurrentHP;
    }

    public void Clear()
    {
        enemyStats = null;
        hpSlider.value = 0f;
    }
}
