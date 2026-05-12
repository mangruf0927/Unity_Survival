using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject prefab;

    [SerializeField] private float placeDistance;

    private GameObject preview;
    private bool isPlacing;
    private bool canPlace;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame) StartPlacement();
        if (!isPlacing) return;

        UpdatePosition();

        if (Mouse.current.leftButton.wasPressedThisFrame) PlaceObject();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) CanclePlacement();
    }

    private void StartPlacement()
    {
        if (isPlacing) return;

        preview = Instantiate(prefab);
        SetPreviewObject(preview);

        isPlacing = true;
    }

    private void UpdatePosition()
    {
        Vector3 forward = player.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetPosition = player.position + forward * placeDistance;
        Vector3 rayOrigin = new Vector3(targetPosition.x, 50f, targetPosition.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                canPlace = false;
                return;
            }

            preview.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(forward));
            canPlace = true;
        }
        else
        {
            canPlace = false;
        }
    }

    private void PlaceObject()
    {
        if (!canPlace) return;

        Instantiate(prefab, preview.transform.position, preview.transform.rotation);
        Destroy(preview);
        ClearPlacement();
    }

    private void CanclePlacement()
    {
        Destroy(preview);
        ClearPlacement();
    }

    private void ClearPlacement()
    {
        preview = null;
        isPlacing = false;
        canPlace = false;
    }

    private void SetPreviewObject(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders) col.enabled = false;
    }
}
