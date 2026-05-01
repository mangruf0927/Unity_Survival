using UnityEngine;

public class Grinder : MonoBehaviour
{
    private int wood = 0;
    private int iron = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Item")) return;

        Item item = other.GetComponent<Item>();
        if (item == null) return;

        ItemDataTable data = item.Data;
        if (data.ItemType == ItemType.MATERIAL || data.HasProperty(ItemProperty.MATERIAL))
        {
            int count = data.MaterialData.GrindCount;

            if (data.MaterialData.Type == MaterialType.WOOD) wood += count;
            else if (data.MaterialData.Type == MaterialType.IRON) iron += count;
            else return;

            Destroy(item.gameObject);
            Debug.Log("나무 : " + wood + ", 철 : " + iron);
        }
    }
}
