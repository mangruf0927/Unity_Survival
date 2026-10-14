using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField, Min(0.01f)] private float progressSpeed = 0.5f;

    private float targetProgress;
    private float currentProgress;
    private bool hasError;

    public bool IsComplete => currentProgress >= 1f;

    private void Awake()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        if (hasError) return;

        currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, progressSpeed * Time.unscaledDeltaTime);
        UpdateDisplay();
    }

    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Max(targetProgress, Mathf.Clamp01(progress));
    }

    private void UpdateDisplay()
    {
        progressSlider.value = currentProgress;
        percentText.text = $"{Mathf.FloorToInt(currentProgress * 100f)}%";
    }

    public void ShowError()
    {
        hasError = true;
        percentText.text = "로딩 실패";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
