using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Chest : WorldObject, IInteractable
{
    private const string AnimationName = "Open";

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Animator animator;

    [SerializeField] private List<int> itemIdList;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Transform uiPoint;

    [SerializeField] private bool useManualOpen;
    [SerializeField] private Vector3 openedLidEuler = new(-90f, 0f, 0f);
    [SerializeField] private float manualOpenDuration = 0.25f;

    private bool isOpened;
    private Transform lidTransform;
    private Quaternion closedLidRotation;
    private bool hasClosedLidRotation;
    private Coroutine manualOpenCoroutine;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

    private void Awake()
    {
        CacheLidTransform();
    }

    public override ObjectSaveData CreateSaveData()
    {
        ObjectSaveData data = base.CreateSaveData();

        data.chestSaveData = new ChestSaveData
        {
            isOpened = isOpened
        };

        return data;
    }

    public override void LoadSaveData(ObjectSaveData data)
    {
        base.LoadSaveData(data);

        if (data.chestSaveData == null) return;

        isOpened = data.chestSaveData.isOpened;

        if (isOpened)
        {
            ApplyOpenedState();
        }
    }

    public bool CanInteract(PlayerController player)
    {
        return !isOpened;
    }

    public void Interact(PlayerController player)
    {
        if (isOpened) return;
        Open();
    }

    private void Open()
    {
        isOpened = true;

        if (animator != null)
        {
            animator.SetTrigger(AnimationName);
        }

        if (useManualOpen)
        {
            PlayManualOpen();
        }

        RandomItem();
    }

    private void RandomItem()
    {
        if (spawnPoint == null || itemRegistry == null || itemIdList == null || itemIdList.Count == 0) return;

        int idx = Random.Range(0, itemIdList.Count);
        itemRegistry.SpawnItem(itemIdList[idx], spawnPoint.position, Quaternion.identity);
    }

    private void ApplyOpenedState()
    {
        isOpened = true;

        if (animator != null)
        {
            animator.ResetTrigger(AnimationName);
            animator.Play(AnimationName, 0, 1f);
            animator.Update(0f);
        }

        if (useManualOpen)
        {
            SetManualOpened();
        }
    }

    private void CacheLidTransform()
    {
        if (lidTransform == null)
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child == transform) continue;
                if (!child.name.Contains("Lid")) continue;

                lidTransform = child;
                break;
            }
        }

        if (lidTransform == null || hasClosedLidRotation) return;

        closedLidRotation = lidTransform.localRotation;
        hasClosedLidRotation = true;
    }

    private void PlayManualOpen()
    {
        CacheLidTransform();
        if (lidTransform == null) return;

        if (manualOpenCoroutine != null)
        {
            StopCoroutine(manualOpenCoroutine);
        }

        manualOpenCoroutine = StartCoroutine(ManualOpenRoutine());
    }

    private IEnumerator ManualOpenRoutine()
    {
        Quaternion start = lidTransform.localRotation;
        Quaternion end = closedLidRotation * Quaternion.Euler(openedLidEuler);
        float duration = Mathf.Max(0.01f, manualOpenDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            lidTransform.localRotation = Quaternion.Slerp(start, end, elapsed / duration);
            yield return null;
        }

        lidTransform.localRotation = end;
        manualOpenCoroutine = null;
    }

    private void SetManualOpened()
    {
        CacheLidTransform();
        if (lidTransform == null) return;

        if (manualOpenCoroutine != null)
        {
            StopCoroutine(manualOpenCoroutine);
            manualOpenCoroutine = null;
        }

        lidTransform.localRotation = closedLidRotation * Quaternion.Euler(openedLidEuler);
    }
}
