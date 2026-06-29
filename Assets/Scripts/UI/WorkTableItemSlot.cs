using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkTableItemSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private GameObject woodCost;
    [SerializeField] private GameObject ironCost;
    [SerializeField] private GameObject purchaseLimit;

    [SerializeField] private TextMeshProUGUI woodCostText;
    [SerializeField] private TextMeshProUGUI ironCostText;

    [SerializeField] private TextMeshProUGUI purchaseLimitText;
    [SerializeField] private TextMeshProUGUI soldOutText;

    [SerializeField] private Image lockOverlay;

    private WorkTableItem item;
    private WorkTableInventory inventory;

    public void SetUp(WorkTableItem item, WorkTableInventory inventory)
    {
        this.item = item;
        this.inventory = inventory;

        iconImage.sprite = item.iconImage;
        nameText.text = item.itemName;

        if (item.needWood == 0) woodCost.SetActive(false);
        if (item.needIron == 0) ironCost.SetActive(false);

        woodCostText.text = item.needWood.ToString();
        ironCostText.text = item.needIron.ToString();

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

        UpdateSlot();
    }

    public void UpdateSlot()
    {
        bool unlocked = inventory.IsUnlocked(item);
        bool soldOut = inventory.IsSoldOut(item);

        iconImage.color = unlocked ? Color.white : new Color(0.25f, 0.25f, 0.25f, 1f);
        nameText.color = unlocked ? Color.white : Color.gray;
        woodCostText.color = unlocked ? Color.white : Color.gray;
        ironCostText.color = unlocked ? Color.white : Color.gray;

        woodCost.SetActive(!soldOut && item.needWood > 0);
        ironCost.SetActive(!soldOut && item.needIron > 0);
        soldOutText.gameObject.SetActive(soldOut);

        lockOverlay.gameObject.SetActive(!unlocked);
    }

    public void OnClickSlot()
    {
        if (inventory.IsSoldOut(item))
        {
            Debug.Log($"{item.itemName} 구매 완료된 아이템입니다.");
            return;
        }

        if (!inventory.IsUnlocked(item))
        {
            Debug.Log($"{item.itemName} 구매할 수 없습니다.");
            return;
        }

        if (!inventory.BuyItem(item))
        {
            Debug.Log($"{item.itemName} 구매 재료가 부족합니다.");
            return;
        }
        Debug.Log($"{item.itemName} 구매 완료");
    }
}
