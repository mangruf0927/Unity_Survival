using UnityEngine;

public class PlaceableItem : EquippableItem
{
    [SerializeField] private GameObject placePrefab;

    public GameObject PlacePrefab => placePrefab;

    public override void OnEquip(PlayerController player)
    {
        gameObject.SetActive(true);

        if (!TryGetComponent<ObjectPlacement>(out var placement)) return;

        placement.StartPlacement(this, player);
    }

    public override void OnUnequip(PlayerController player)
    {
        if (TryGetComponent<ObjectPlacement>(out var placement)) placement.CancelPlacement();

        gameObject.SetActive(false);
    }
}
