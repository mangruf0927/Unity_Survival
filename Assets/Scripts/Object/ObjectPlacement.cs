using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private float placeDistance = 2f;

    private PlayerController currentPlayer;
    private PlaceableItem currentItem;
    private GameObject preview;
    private bool isPlacing;
    private bool canPlace;

    private void Update()
    {
        if (!isPlacing) return;

        UpdatePosition();

        if (Mouse.current.leftButton.wasPressedThisFrame) PlaceObject();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) CancelPlacement();
    }

    public void StartPlacement(PlaceableItem item, PlayerController player)
    {
        if (isPlacing) return;
        if (item == null || item.PlacePrefab == null || player == null) return;

        currentItem = item;
        currentPlayer = player;

        preview = Instantiate(currentItem.PlacePrefab);
        SetPreviewObject(preview);

        isPlacing = true;
    }

    private void UpdatePosition()
    {
        if (currentPlayer == null || preview == null) return;

        Vector3 forward = currentPlayer.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetPosition = currentPlayer.transform.position + forward * placeDistance;
        Vector3 rayOrigin = new Vector3(targetPosition.x, 50f, targetPosition.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                canPlace = false;
                return;
            }

            preview.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(forward));
            SnapToGround(preview, hit.point.y);
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

        PlayerController player = currentPlayer;
        PlaceableItem item = currentItem;

        Instantiate(item.PlacePrefab, preview.transform.position, preview.transform.rotation);
        if (preview != null) Destroy(preview);
        ClearPlacement();

        player.ConsumeEquippedItem(item);
    }

    public void CancelPlacement()
    {
        if (preview != null) Destroy(preview);
        ClearPlacement();
    }

    private void ClearPlacement()
    {
        currentPlayer = null;
        currentItem = null;
        preview = null;
        isPlacing = false;
        canPlace = false;
    }

    private void SetPreviewObject(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders) col.enabled = false;
    }

    private void SnapToGround(GameObject obj, float groundY)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        float yOffset = groundY - bounds.min.y;
        obj.transform.position += Vector3.up * yOffset;
    }
}
