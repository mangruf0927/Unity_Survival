using TMPro;
using UnityEngine;

public class WorkTableItemSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;

    private WorkTableItem item;
    private WorkTableInventory inventory;

    public void SetUp(WorkTableItem item, WorkTableInventory inventory)
    {
        this.item = item;
        this.inventory = inventory;

        nameText.text = item.itemName;
        costText.text = $"나무{item.needWood} / 철{item.needIron}";
    }

    public void OnClickSlot()
    {
        if (!inventory.BuyItem(item))
        {
            Debug.Log($"{item.itemName} 구매 재료가 부족합니다.");
            return;
        }
        Debug.Log($"{item.itemName} 구매 완료");
    }
}
