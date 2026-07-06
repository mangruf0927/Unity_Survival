using UnityEngine;
using UnityEngine.UI;

public class CultistHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private CultistStats cultistStats;

    public void SetHPBar(CultistStats stats)
    {
        cultistStats = stats;
        UpdateHPBar();
    }

    public void UpdateHPBar()
    {
        hpSlider.maxValue = cultistStats.MaxHp;
        hpSlider.value = cultistStats.CurrentHp;
    }

    public void Clear()
    {
        cultistStats = null;
        if (hpSlider != null) hpSlider.value = 0f;
    }
}
