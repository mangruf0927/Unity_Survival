using UnityEngine;

public class Door : WorldObject, IInteractable
{
    [SerializeField] private float holdTime = 0.01f;
    [SerializeField] private Transform uiPoint;

    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float moveDuration = 0.1f;

    public float HoldTime => holdTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position;

    private bool isOpened;
    private float elapsedTime;

    private Quaternion startRotation;
    private Quaternion targetRotation;
    private Quaternion closedRotation;

    private void Awake()
    {
        if (doorPivot == null) return;
        closedRotation = doorPivot.localRotation;
    }

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

    public override ObjectSaveData CreateSaveData()
    {
        ObjectSaveData data = base.CreateSaveData();

        data.doorSaveData = new DoorSaveData
        {
            isOpened = isOpened
        };

        return data;
    }

    public override void LoadSaveData(ObjectSaveData data)
    {
        base.LoadSaveData(data);

        isOpened = data.doorSaveData.isOpened;

        doorPivot.localRotation = isOpened ? closedRotation * Quaternion.Euler(0f, 0f, openAngle) : closedRotation;
    }

    public bool CanInteract(PlayerController player)
    {
        return !isOpened && doorPivot != null;
    }

    public void Interact(PlayerController player)
    {
        if (isOpened || doorPivot == null) return;

        isOpened = true;
        elapsedTime = 0f;

        startRotation = doorPivot.localRotation;
        targetRotation = startRotation * Quaternion.Euler(0f, 0f, openAngle);
    }
}
