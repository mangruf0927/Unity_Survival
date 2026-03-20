using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EquippedItemUI itemUI;

    private void Start()
    {
        player.OnEquipped += EquipperItem;
    }

    private void OnDestroy()
    {
        player.OnEquipped -= EquipperItem;
    }

    private void EquipperItem(EquippableItem item)
    {
        itemUI.UpdateUI(item);
    }
}
