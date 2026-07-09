using UnityEngine;
using System.Collections.Generic;

public class Chest : WorldObject, IInteractable
{
    private const string AnimationName = "Open";

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Animator animator;

    [SerializeField] private List<int> itemIdList;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Transform uiPoint;

    private bool isOpened;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

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

        if (animator == null) return;

        animator.ResetTrigger(AnimationName);
        animator.Play(AnimationName, 0, 1f);
        animator.Update(0f);
    }
}
