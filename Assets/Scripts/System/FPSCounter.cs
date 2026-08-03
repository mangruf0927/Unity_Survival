using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.5f;

    private float elapsedTime;
    private int frameCount;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (elapsedTime < updateInterval)
            return;

        float fps = frameCount / elapsedTime;
        float milliseconds = elapsedTime / frameCount * 1000f;

        fpsText.text = $"{fps:F1} FPS ({milliseconds:F1} ms)";

        elapsedTime = 0f;
        frameCount = 0;
    }
}