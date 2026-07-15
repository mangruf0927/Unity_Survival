using UnityEngine;

public class CampFireMarker : MonoBehaviour
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private SafeZone safeZone;
    [SerializeField] private Transform player;
    [SerializeField] private Transform markerPoint;

    [SerializeField] private GameObject markerUI;
    [SerializeField] private RectTransform markerRect;

    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float minScale = 0.5f;

    [SerializeField] private float scaleDistance = 50f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (markerUI != null)
        {
            markerUI.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        ShowUI();
    }

    private void ShowUI()
    {
        if (campFire == null || safeZone == null || player == null || markerPoint == null ||
            markerUI == null || markerRect == null || mainCamera == null)
        {
            HideMarker();
            return;
        }

        bool isBurning = campFire.IsBurning;
        bool isInside = safeZone.IsInside(player.position);

        if (!isBurning || isInside)
        {
            HideMarker();
            return;
        }

        UpdateMarker();
    }

    private void UpdateMarker()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(markerPoint.position);

        bool isBehind = viewportPosition.z <= 0f;
        bool isOutside = viewportPosition.x < 0f || viewportPosition.x > 1f ||
                         viewportPosition.y < 0f || viewportPosition.y > 1f;

        if (isBehind || isOutside)
        {
            HideMarker();
            return;
        }

        ShowMarker();
        UpdateScale();
    }

    private void UpdateScale()
    {
        Vector3 playerPosition = player.position;
        Vector3 campFirePosition = campFire.transform.position;

        playerPosition.y = 0f;
        campFirePosition.y = 0f;

        float distance = Vector3.Distance(playerPosition, campFirePosition);
        float scaleRatio = Mathf.InverseLerp(safeZone.CurrentRadius, scaleDistance, distance);
        float scale = Mathf.Lerp(maxScale, minScale, scaleRatio);

        markerRect.localScale = Vector3.one * scale;
    }

    private void ShowMarker()
    {
        if (!markerUI.activeSelf)
        {
            markerUI.SetActive(true);
        }
    }

    private void HideMarker()
    {
        if (markerUI != null && markerUI.activeSelf)
        {
            markerUI.SetActive(false);
        }
    }
}
