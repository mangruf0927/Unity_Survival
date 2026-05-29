using UnityEngine;

public class PlaceableItem : EquippableItem
{
    [SerializeField] private GameObject placePrefab;

    public GameObject PlacePrefab => placePrefab;

    public override void OnEquip(PlayerController player)
    {
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        gameObject.SetActive(false);
    }
}
