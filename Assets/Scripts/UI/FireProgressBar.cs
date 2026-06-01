using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampFireProgressUI : MonoBehaviour, IObserver
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TextMeshProUGUI fuelText;

    private void Start()
    {
        if (campFire == null) return;
        campFire.AddObserver(this);
        Notify();
    }

    private void OnDestroy()
    {
        if (campFire == null) return;
        campFire.RemoveObserver(this);
    }

    public void Notify()
    {
        if (campFire == null) return;

        float ratio = campFire.NeedFuel <= 0 ? 0f : Mathf.Clamp01((float)campFire.CurrentFuel / campFire.NeedFuel);
        int displayFuel = Mathf.RoundToInt(ratio * 100);

        if (fuelSlider != null)
        {
            fuelSlider.maxValue = 100;
            fuelSlider.value = displayFuel;
        }

        if (fuelText != null)
        {
            fuelText.text = $"level {campFire.CurrentLevel} progress : {displayFuel} / {100}";
        }
    }
}