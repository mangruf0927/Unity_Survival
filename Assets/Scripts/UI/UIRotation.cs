using UnityEngine;

public class UIRotation : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        if (cam == null) return;
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
