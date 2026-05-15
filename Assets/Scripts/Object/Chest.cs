using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private float openTime = 3f;
    [SerializeField] private Animator animator;

    [SerializeField] private List<GameObject> itemList;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Transform uiPoint;

    private bool isOpened;

    public float HoldTime => openTime;
    public Vector3 UIPosition => uiPoint != null ? uiPoint.position : transform.position + Vector3.up * 3f;

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
            animator.SetTrigger("Open");
        }

        RandomItem();
    }

    private void RandomItem()
    {
        if (itemList == null || itemList.Count == 0) return;
        if (spawnPoint == null) return;

        int idx = Random.Range(0, itemList.Count);
        Instantiate(itemList[idx], spawnPoint.position, Quaternion.identity);
    }
}