using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightLighting : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private Light sunLight;

    [SerializeField] private float transitionDuration;
    [SerializeField] private float nightSunIntensity;
    [SerializeField] private Color nightAmbientColor;
    [SerializeField] private Color nightSunColor;

    private float daySunIntensity;
    private Color dayAmbientColor;
    private Color daySunColor;
    private AmbientMode dayAmbientMode;

    private CancellationTokenSource cts;

    private void Awake()
    {
        daySunIntensity = sunLight.intensity;
        dayAmbientColor = RenderSettings.ambientLight;
        daySunColor = sunLight.color;
        dayAmbientMode = RenderSettings.ambientMode;
    }

    private void OnEnable()
    {
        timeSystem.OnPhaseChanged += PhaseChanged;
    }

    private void OnDisable()
    {
        timeSystem.OnPhaseChanged -= PhaseChanged;
        CancelTransition();
    }

    private void PhaseChanged(Phase phase, int dayCount)
    {
        CancelTransition();

        cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        TransitionLightingAsync(phase, cts.Token).Forget();
    }

    private async UniTask TransitionLightingAsync(Phase phase, CancellationToken ct)
    {
        bool isNight = phase == Phase.NIGHT;

        float startIntensity = sunLight.intensity;
        Color startSunColor = sunLight.color;
        Color startAmbientColor = RenderSettings.ambientLight;

        float targetIntensity = isNight ? nightSunIntensity : daySunIntensity;
        Color targetSunColor = isNight ? nightSunColor : daySunColor;
        Color targetAmbientColor = isNight ? nightAmbientColor : dayAmbientColor;
        AmbientMode targetAmbientMode = isNight ? AmbientMode.Flat : dayAmbientMode;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);

            t = Mathf.SmoothStep(0f, 1f, t);

            sunLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            sunLight.color = Color.Lerp(startSunColor, targetSunColor, t);
            RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetAmbientColor, t);

            bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            if (canceled) return;
        }

        sunLight.intensity = targetIntensity;
        sunLight.color = targetSunColor;
        RenderSettings.ambientLight = targetAmbientColor;
        RenderSettings.ambientMode = targetAmbientMode;
    }

    private void CancelTransition()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }
}