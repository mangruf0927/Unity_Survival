using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampFireProgressUI : MonoBehaviour, IObserver
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private SafeZone safeZone;
    [SerializeField] private Transform player;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TextMeshProUGUI fuelText;

    private void Start()
    {
        if (campFire == null)
        {
            SetActive(false);
            return;
        }

        campFire.AddObserver(this);
        Notify();
    }

    private void Update()
    {
        bool canShow = campFire != null && safeZone != null && player != null &&
                       campFire.IsBurning && safeZone.IsInside(player.position);

        SetActive(canShow);
    }

    private void OnDestroy()
    {
        if (campFire == null) return;
        campFire.RemoveObserver(this);
    }

    public void Notify()
    {
        if (campFire == null) return;

        if (campFire.IsLevelingUp)
        {
            if (fuelSlider != null)
            {
                fuelSlider.maxValue = 100;
                fuelSlider.value = 100;
            }

            if (fuelText != null)
            {
                fuelText.text = $"화재레벨 {campFire.CurrentLevel}. 맵 확장";
            }

            return;
        }

        if (fuelSlider != null)
        {
            fuelSlider.maxValue = 100;
            fuelSlider.value = campFire.CurrentFuel;
        }

        if (fuelText != null)
        {
            fuelText.text = $"level {campFire.CurrentLevel} progress : {(int)campFire.CurrentFuel} / 100";
        }
    }

    private void SetActive(bool isActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.activeSelf != isActive) child.SetActive(isActive);
        }
    }
}
