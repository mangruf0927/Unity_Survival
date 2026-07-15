using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private float holdTime = 0.01f;
    [SerializeField] private Transform uiPoint;

    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float moveDuration = 0.1f;

    public float HoldTime => holdTime;
    public Vector3 UIPosition => uiPoint.position;

    private bool isOpened;
    private float elapsedTime;

    private Quaternion startRotation;
    private Quaternion targetRotation;

    private void Update()
    {
        if (!isOpened) return;
        if (elapsedTime >= moveDuration) return;

        elapsedTime += Time.deltaTime;

        float duration = Mathf.Max(0.01f, moveDuration);
        float t = Mathf.Clamp01(elapsedTime / duration);
        t = Mathf.SmoothStep(0f, 1f, t);

        doorPivot.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);

        if (elapsedTime >= duration) doorPivot.localRotation = targetRotation;
    }

    public bool CanInteract(PlayerController player)
    {
        return !isOpened;
    }

    public void Interact(PlayerController player)
    {
        if (isOpened) return;

        isOpened = true;
        elapsedTime = 0f;

        startRotation = doorPivot.localRotation;
        targetRotation = startRotation * Quaternion.Euler(0f, 0f, openAngle);
    }
}
