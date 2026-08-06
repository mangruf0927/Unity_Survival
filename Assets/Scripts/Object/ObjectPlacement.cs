using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private ObjectRegistry objectRegistry;
    [SerializeField] private LayerMask groundLayerMask = ~0;
    [SerializeField] private float placeDistance = 2f;

    private PlayerController currentPlayer;
    private PlaceableItem currentItem;
    private GameObject previewPrefab;

    private float previewOffset;

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

        previewPrefab = Instantiate(currentItem.PlacePrefab);

        SetPreviewObject(previewPrefab);
        CacheGroundOffset(previewPrefab);

        isPlacing = true;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (currentPlayer == null || previewPrefab == null) return;

        Vector3 forward = currentPlayer.transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude <= Mathf.Epsilon)
        {
            canPlace = false;
            return;
        }

        forward.Normalize();

        Vector3 targetPosition = currentPlayer.transform.position + forward * placeDistance;
        Vector3 rayOrigin = new(targetPosition.x, 50f, targetPosition.z);

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 50f, groundLayerMask, QueryTriggerInteraction.Ignore))
        {
            canPlace = false;
            return;
        }

        previewPrefab.transform.SetPositionAndRotation(hit.point + Vector3.up * previewOffset, Quaternion.LookRotation(forward));
        canPlace = true;
    }

    private void CacheGroundOffset(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            previewOffset = 0f;
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        previewOffset = obj.transform.position.y - bounds.min.y;
    }

    private void PlaceObject()
    {
        if (!canPlace) return;

        PlayerController player = currentPlayer;
        PlaceableItem item = currentItem;

        GameObject placedObject = Instantiate(item.PlacePrefab, previewPrefab.transform.position, previewPrefab.transform.rotation);
        RegisterPlacedObject(placedObject);

        Bed bed = placedObject.GetComponentInChildren<Bed>();
        if (bed != null)
        {
            bed.OnPlaced(timeSystem);
        }

        SunDial sunDial = placedObject.GetComponentInChildren<SunDial>();
        if (sunDial != null)
        {
            sunDial.OnPlaced(timeSystem);
        }

        Destroy(previewPrefab);
        ClearPlacement();

        player.ConsumeEquippedItem(item);
    }

    private void RegisterPlacedObject(GameObject placedObject)
    {
        if (objectRegistry == null || placedObject == null) return;

        WorldObject worldObject = placedObject.GetComponentInChildren<WorldObject>();
        if (worldObject == null) return;

        objectRegistry.RegisterRuntime(worldObject);
    }

    public void CancelPlacement()
    {
        if (previewPrefab != null) Destroy(previewPrefab);
        ClearPlacement();
    }

    private void ClearPlacement()
    {
        currentPlayer = null;
        currentItem = null;
        previewPrefab = null;
        previewOffset = 0f;
        isPlacing = false;
        canPlace = false;
    }

    private void SetPreviewObject(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders) col.enabled = false;
    }
}
