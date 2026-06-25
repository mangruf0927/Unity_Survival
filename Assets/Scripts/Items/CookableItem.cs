using UnityEngine;

public class CookableItem : MonoBehaviour
{
    [SerializeField] private Item cookedPrefab;

    private bool isCooked;

    private void OnTriggerEnter(Collider other)
    {
        Cook(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Cook(other);
    }

    private void Cook(Collider other)
    {
        if (isCooked) return;

        CampFire campFire = other.GetComponentInParent<CampFire>();
        if (campFire == null || !campFire.IsBurning) return;

        Item rawItem = GetComponent<Item>();
        if (rawItem == null || rawItem.Data?.FoodData == null) return;
        if (!rawItem.Data.FoodData.NeedCook) return;

        isCooked = true;

        Rigidbody rawRigidbody = rawItem.GetComponent<Rigidbody>();
        Item cookedItem = Instantiate(cookedPrefab, rawItem.transform.position, rawItem.transform.rotation);
        Rigidbody cookedRigidbody = cookedItem.GetComponent<Rigidbody>();

        if (rawRigidbody != null && cookedRigidbody != null)
        {
            cookedRigidbody.linearVelocity = rawRigidbody.linearVelocity;
            cookedRigidbody.angularVelocity = rawRigidbody.angularVelocity;
        }
        Destroy(rawItem.gameObject);
    }
}
