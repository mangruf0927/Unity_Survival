using UnityEngine;

public class Grinder : MonoBehaviour
{
    [SerializeField] private WorkTableInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponentInParent<Item>();
        if (item == null) return;

        if (item.gameObject.layer != LayerMask.NameToLayer("Item")) return;

        ItemData data = item.Data;
        if (data.MaterialData == null) return;

        if (data.ItemType == ItemType.MATERIAL || data.HasProperty(ItemProperty.MATERIAL))
        {
            int count = data.MaterialData.GrindCount;

            if (data.MaterialData.Type == MaterialType.WOOD)
            {
                inventory.AddMaterial(MaterialType.WOOD, count);
            }
            else if (data.MaterialData.Type == MaterialType.IRON)
            {
                inventory.AddMaterial(MaterialType.IRON, count);
            }
            else return;

            Destroy(item.gameObject);       // pool 쓰는 경우 나중에 ReturnToPool로 변경
        }
    }
}
