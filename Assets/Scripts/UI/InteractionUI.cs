using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionKey;

    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        Hide();
    }

    public void Show(Vector3 position)
    {
        interactionKey.SetActive(true);
        UpdatePosition(position);
    }

    public void Hide()
    {
        interactionKey.SetActive(false);
    }

    private void UpdatePosition(Vector3 worldPosition)
    {
        if (mainCamera == null || rectTransform == null) return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
        {
            interactionKey.SetActive(false);
            return;
        }

        rectTransform.position = screenPosition;
    }
}
