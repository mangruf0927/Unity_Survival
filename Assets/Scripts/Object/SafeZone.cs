using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private GameObject linePrefab;

    [SerializeField] private float startRadius;
    [SerializeField] private float radiusGap;
    [SerializeField] private int lineCount;

    private float currentRadius;
    private bool isSafe;
    private GameObject safeLine;

    public float CurrentRadius => currentRadius;
    public bool IsSafe => isSafe;

    private void OnEnable()
    {
        if (campFire == null) return;

        campFire.OnLevelUp += UpdateRadius;
        campFire.OnFireChanged += UpdateSafeZone;

        UpdateRadius(campFire.CurrentLevel);
        UpdateSafeZone(campFire.IsBurning);
    }

    private void OnDisable()
    {
        if (campFire == null) return;

        campFire.OnLevelUp -= UpdateRadius;
        campFire.OnFireChanged -= UpdateSafeZone;
    }

    private void UpdateRadius(int level)
    {
        currentRadius = startRadius + level * radiusGap;

        if (safeLine == null)
        {
            CreateLine();
        }

        UpdateLine();
    }

    private void UpdateSafeZone(bool isBurning)
    {
        isSafe = isBurning;
        if (safeLine != null) safeLine.SetActive(isBurning);
    }

    private void CreateLine()
    {
        if (linePrefab == null) return;

        safeLine = new GameObject("SafeLine");
        safeLine.transform.SetParent(transform);

        for (int i = 0; i < lineCount; i++)
        {
            Instantiate(linePrefab, safeLine.transform);
        }
    }

    private void UpdateLine()
    {
        if (safeLine == null) return;

        float angleGap = 360f / lineCount;
        float lineLength = 2f * Mathf.PI * CurrentRadius / lineCount * 0.5f;

        for (int i = 0; i < lineCount; i++)
        {
            float angle = angleGap * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * CurrentRadius;
            float z = Mathf.Cos(radian) * CurrentRadius;

            Transform line = safeLine.transform.GetChild(i);
            line.localScale = new Vector3(lineLength, 0.02f, 0.1f);
            line.position = transform.position + new Vector3(x, 0.52f, z);
            line.rotation = Quaternion.LookRotation(new Vector3(x, 0f, z));
        }

        safeLine.SetActive(IsSafe);
    }

    public bool IsInside(Vector3 position)
    {
        if (!IsSafe) return false;

        Vector3 direction = position - transform.position;
        direction.y = 0f;

        return direction.sqrMagnitude <= CurrentRadius * CurrentRadius;
    }

    public Vector3 GetBoundaryPosition(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        return transform.position + direction * CurrentRadius;
    }
}
