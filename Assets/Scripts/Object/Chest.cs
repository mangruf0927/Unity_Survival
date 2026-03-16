using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private float openTime;
    [SerializeField] private Animator animator;

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
