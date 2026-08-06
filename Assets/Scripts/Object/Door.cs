using System.Threading;
using Cysharp.Threading.Tasks;
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
    private Quaternion closedRotation;

    private CancellationTokenSource cts;

    private void Awake()
    {
        if (doorPivot == null) return;
        closedRotation = doorPivot.localRotation;
    }

    private void OnDisable()
    {
        CancelOpen();
    }

    public bool CanInteract(PlayerController player)
    {
        return !isOpened && doorPivot != null;
    }

    public void Interact(PlayerController player)
    {
        if (isOpened || doorPivot == null) return;

        isOpened = true;
        CancelOpen();

        cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        OpenAsync(cts.Token).Forget();
    }

    private async UniTask OpenAsync(CancellationToken ct)
    {
        Quaternion startRotation = doorPivot.localRotation;
        Quaternion targetRotation = closedRotation * Quaternion.Euler(0f, 0f, openAngle);

        float duration = Mathf.Max(0.01f, moveDuration);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            doorPivot.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
            if (canceled) return;
        }

        doorPivot.localRotation = targetRotation;
    }

    private void CancelOpen()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    // Save/Load
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
        isOpened = data.doorSaveData.isOpened;
        if (doorPivot == null) return;
        doorPivot.localRotation = isOpened ? closedRotation * Quaternion.Euler(0f, 0f, openAngle) : closedRotation;
    }
}
