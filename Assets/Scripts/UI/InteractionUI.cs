using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject interactionKey;
    [SerializeField] private Image ProgressImage;

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
        SetProgress(0f);
    }

    public void SetProgress(float progress)
    {
        ProgressImage.fillAmount = Mathf.Clamp01(progress);
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
