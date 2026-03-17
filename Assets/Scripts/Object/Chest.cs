using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private float openTime;
    [SerializeField] private Animator animator;

    [SerializeField] private List<GameObject> itemLists;
    [SerializeField] private Transform spawnPoint;

    private float holdTime;
    private bool isOpened = false;
    private bool isPlayerCollision = false;

    public void Hold()
    {
        if(isOpened || !isPlayerCollision) return;

        holdTime += Time.deltaTime;

        if(holdTime >= openTime) Open();
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
        int idx = Random.Range(0, itemLists.Count);
        
        Instantiate(itemLists[idx], spawnPoint.position, Quaternion.identity);
    }

    public void Cancel()
    {
        if(isOpened) return;
        holdTime = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("E키를 3초 누르세요");
            isPlayerCollision = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerCollision = false;
            holdTime = 0f;
        }
    }
}
