using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private float openTime;
    [SerializeField] private Animator animator;

    [SerializeField] private List<GameObject> itemList;
    [SerializeField] private Transform spawnPoint;

    private float holdTime;
    private bool isOpened = false;
    private bool isPlayerCollision = false;

    public void Hold()
    {
        if (isOpened || !isPlayerCollision) return;

        holdTime += Time.deltaTime;

        if (holdTime >= openTime) Open();
    }

    private void Open()
    {
        isOpened = true;
        holdTime = 0f;

        animator.SetTrigger("Open");
        RandomItem();
    }

    private void RandomItem()
    {
        if (itemList == null || itemList.Count == 0) return;

        int idx = Random.Range(0, itemList.Count);
        Instantiate(itemList[idx], spawnPoint.position, Quaternion.identity);
    }

    public void Cancel()
    {
        if (isOpened) return;
        holdTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("E키를 3초 누르세요");
            isPlayerCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerCollision = false;
            holdTime = 0f;
        }
    }
}
