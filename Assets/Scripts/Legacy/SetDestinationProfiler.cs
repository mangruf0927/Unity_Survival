using UnityEngine;

public class SetDestinationProfiler : MonoBehaviour
{
    public static int Count { get; private set; }

    private const float MeasureTime = 10f;

    private static float timer;
    private static bool isMeasuring;

    public static void Record()
    {
        if (!isMeasuring) return;
        Count++;
    }

    public static void StartMeasure()
    {
        Count = 0;
        timer = 0f;
        isMeasuring = true;

        Debug.Log("SetDestination 측정 시작");
    }

    private void Update()
    {
        if (!isMeasuring) return;

        timer += Time.unscaledDeltaTime;
        if (timer < MeasureTime) return;

        isMeasuring = false;

        float callsPerSecond = Count / timer;
        Debug.Log($"SetDestination 측정 완료 / {timer:F2}초 / 총 {Count}회 / 평균 {callsPerSecond:F2} calls/sec");
    }
}