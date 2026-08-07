using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementTest : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject testPrefab;

    private GameObject previewObject;
    private bool isPlacing;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame) StartPlacement();
        if (!isPlacing) return;

        UpdatePosition();

        if (Mouse.current.leftButton.wasPressedThisFrame) PlaceObject();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) CancelPlacement();
    }

    private void StartPlacement()
    {
        if (isPlacing) return;

        previewObject = Instantiate(testPrefab);
        SetPreviewObject(previewObject, true);

        isPlacing = true;
    }

    private void UpdatePosition()
    {
        Vector3 position = player.position + player.forward * 2;
        previewObject.transform.SetPositionAndRotation(position, Quaternion.LookRotation(player.forward));
    }

    private void PlaceObject()
    {
        Instantiate(testPrefab, previewObject.transform.position, previewObject.transform.rotation);
        Destroy(previewObject);
        ClearPlacement();
    }

    private void CancelPlacement()
    {
        Destroy(previewObject);
        ClearPlacement();
    }

    private void ClearPlacement()
    {
        previewObject = null;
        isPlacing = false;
    }

    private void SetPreviewObject(GameObject obj, bool isPreview)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = !isPreview;
    }
}

/*
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementTest : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject testPrefab;

    [Header("Placement")]
    [SerializeField] private float placeDistance = 2f;
    [SerializeField] private float rayStartHeight = 5f;
    [SerializeField] private float rayDistance = 10f;

    [Header("Tag")]
    [SerializeField] private string groundTag = "Ground";

    private GameObject previewObject;
    private bool isPlacing;
    private bool canPlace;

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
            StartPlacement();

        if (!isPlacing) return;

        UpdatePosition();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            PlaceObject();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            CancelPlacement();
    }

    private void StartPlacement()
    {
        if (isPlacing) return;

        previewObject = Instantiate(testPrefab);

        SetPreviewObject(previewObject, true);

        isPlacing = true;
    }

    private void UpdatePosition()
    {
        Vector3 targetPosition = player.position + player.forward * placeDistance;
        Vector3 rayOrigin = targetPosition + Vector3.up * rayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            if (!hit.collider.CompareTag(groundTag))
            {
                canPlace = false;
                return;
            }

            Vector3 forward = player.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.01f)
                forward = player.transform.forward;

            Quaternion rotation = Quaternion.LookRotation(forward.normalized);
            previewObject.transform.rotation = rotation;

            float bottomOffset = GetBottomOffset(previewObject);

            Vector3 placePosition = hit.point;
            placePosition.y += bottomOffset;

            previewObject.transform.position = placePosition;

            canPlace = CheckCanPlace();
        }
        else
        {
            canPlace = false;
        }
    }

    private void PlaceObject()
    {
        if (!canPlace)
        {
            Debug.Log("설치할 수 없는 위치입니다.");
            return;
        }

        Instantiate(testPrefab, previewObject.transform.position, previewObject.transform.rotation);

        Destroy(previewObject);
        ClearPlacement();
    }

    private void CancelPlacement()
    {
        Destroy(previewObject);
        ClearPlacement();
    }

    private void ClearPlacement()
    {
        previewObject = null;
        isPlacing = false;
        canPlace = false;
    }

    private void SetPreviewObject(GameObject obj, bool isPreview)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = !isPreview;
        }
    }

    private float GetBottomOffset(GameObject obj)
    {
        Bounds bounds = GetRendererBounds(obj);

        return obj.transform.position.y - bounds.min.y;
    }

    private bool CheckCanPlace()
    {
        Bounds bounds = GetRendererBounds(previewObject);

        Collider[] hits = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            Quaternion.identity,
            (int)QueryTriggerInteraction.Ignore
        );

        return true;
    }

    private Bounds GetRendererBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"{obj.name}에 Renderer가 없습니다.");
            return new Bounds(obj.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}
*/