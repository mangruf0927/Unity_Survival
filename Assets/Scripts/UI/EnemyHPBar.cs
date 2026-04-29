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
        hpSlider.maxValue = enemyStats.MaxHp;
        hpSlider.value = enemyStats.CurrentHp;
    }

    public void Clear()
    {
        enemyStats = null;
        if (hpSlider != null) hpSlider.value = 0f;
    }
}
