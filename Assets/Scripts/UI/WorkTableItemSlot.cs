using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkTableItemSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private GameObject ironCost;
    [SerializeField] private GameObject woodCost;
    [SerializeField] private GameObject purchaseLimit;

    [SerializeField] private TextMeshProUGUI ironCostText;
    [SerializeField] private TextMeshProUGUI woodCostText;

    [SerializeField] private TextMeshProUGUI purchaseLimitText;
    [SerializeField] private TextMeshProUGUI soldOutText;

    [SerializeField] private Image lockOverlay;

    private WorkTableItem item;
    private WorkTableInventory inventory;
    private WorkTableInventoryUI inventoryUI;

    public void SetUp(WorkTableItem item, WorkTableInventory inventory, WorkTableInventoryUI inventoryUI)
    {
        this.item = item;
        this.inventory = inventory;
        this.inventoryUI = inventoryUI;

        iconImage.sprite = item.iconImage;
        nameText.text = item.itemName;

        ironCostText.text = item.needIron.ToString();
        woodCostText.text = item.needWood.ToString();

        SetPurchaseLimit();
        UpdateSlot();
    }

    private void SetPurchaseLimit()
    {
        if (item.purchaseLimit < 0)
        {
            purchaseLimitText.text = "∞";
            purchaseLimit.SetActive(true);
        }
        else if (item.purchaseLimit > 1)
        {
            purchaseLimitText.text = $"x{item.purchaseLimit}";
            purchaseLimit.SetActive(true);
        }
        else
        {
            purchaseLimit.SetActive(false);
        }
    }

    public void UpdateSlot()
    {
        bool unlocked = inventory.IsUnlocked(item);
        bool soldOut = inventory.IsSoldOut(item);

        iconImage.color = unlocked ? Color.white : new Color(0.25f, 0.25f, 0.25f, 1f);
        nameText.color = unlocked ? Color.white : Color.gray;
        ironCostText.color = unlocked ? Color.white : Color.gray;
        woodCostText.color = unlocked ? Color.white : Color.gray;

        ironCost.SetActive(!soldOut && item.needIron > 0);
        woodCost.SetActive(!soldOut && item.needWood > 0);
        soldOutText.gameObject.SetActive(soldOut);

        lockOverlay.gameObject.SetActive(!unlocked);
    }

    public void OnClickSlot()
    {
        inventoryUI.SelectItem(item);
    }
}
