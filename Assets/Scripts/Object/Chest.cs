using UnityEngine;
using System.Collections.Generic;

public class Chest : WorldObject, IInteractable
{
    private const string AnimationName = "Open";

    [SerializeField] private float openTime = 3f;
    [SerializeField] private Animator animator;

    [SerializeField] private List<int> itemIdList;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform uiPoint;

    private bool isOpened;
    private EquippableRegistry equippableRegistry;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

    public void Initialize(ItemRegistry itemRegistry, EquippableRegistry equippableRegistry)
    {
        Initialize(itemRegistry);
        this.equippableRegistry = equippableRegistry;
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
        if (spawnPoint == null || itemIdList == null || itemIdList.Count == 0) return;

        int index = Random.Range(0, itemIdList.Count);
        int itemId = itemIdList[index];

        if (equippableRegistry != null)
        {
            EquippableItem equippable = equippableRegistry.SpawnItem(itemId, spawnPoint.position, spawnPoint.rotation);

            if (equippable != null)
            {
                equippable.Detach();
                return;
            }
        }

        if (ItemRegistry != null)
        {
            Item item = ItemRegistry.SpawnItem(itemId, spawnPoint.position, spawnPoint.rotation);

            if (item != null)
            {
                return;
            }
        }
    }

    private void ApplyOpenedState()
    {
        if (animator == null) return;

        animator.Play(AnimationName, 0, 1f);
        animator.Update(0f);
    }

    // Save/Load
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
}
