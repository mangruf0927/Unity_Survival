using UnityEngine;
using UnityEngine.Rendering;

public class DayNightLighting : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private Light sunLight;

    [SerializeField] private float transitionSpeed;
    [SerializeField] private float nightSunIntensity;
    [SerializeField] private Color nightAmbientColor;
    [SerializeField] private Color nightSunColor;

    private float daySunIntensity;
    private Color dayAmbientColor;
    private Color daySunColor;
    private AmbientMode dayAmbientMode;

    private void Start()
    {
        daySunIntensity = sunLight.intensity;
        dayAmbientColor = RenderSettings.ambientLight;
        daySunColor = sunLight.color;
        dayAmbientMode = RenderSettings.ambientMode;
    }

    private void Update()
    {
        bool isNight = timeSystem.CurPhase == Phase.NIGHT;

        float targetSunIntensity = isNight ? nightSunIntensity : daySunIntensity;
        Color targetAmbientColor = isNight ? nightAmbientColor : dayAmbientColor;
        Color targetSunColor = isNight ? nightSunColor : daySunColor;

        RenderSettings.ambientMode = isNight ? AmbientMode.Flat : dayAmbientMode;

        float t = Time.deltaTime * transitionSpeed;

        sunLight.intensity = Mathf.Lerp(sunLight.intensity, targetSunIntensity, t);
        sunLight.color = Color.Lerp(sunLight.color, targetSunColor, t);
        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetAmbientColor, t);
    }
}