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