using UnityEngine;

public class Grinder : MonoBehaviour
{
    [SerializeField] private WorkTableInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Item")) return;

        Item item = other.GetComponent<Item>();
        if (item == null) return;

        ItemData data = item.Data;
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
