using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkTableInventoryUI : MonoBehaviour, IObserver
{
    [SerializeField] private WorkTableInventory inventory;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Detail")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject ironCost;
    [SerializeField] private GameObject woodCost;
    [SerializeField] private TextMeshProUGUI ironCostText;
    [SerializeField] private TextMeshProUGUI woodCostText;
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftText;
    [SerializeField] private Color lackMaterialColor;

    [Header("Slot")]
    [SerializeField] private WorkTableItemSlot slotPrefab;
    [SerializeField] private Transform level1Parent;
    [SerializeField] private Transform level2Parent;
    [SerializeField] private Transform level3Parent;

    private readonly List<WorkTableItemSlot> slots = new();
    private WorkTableItemSlot selectedSlot;
    private WorkTableItem selectedItem;

    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        inventory.AddObserver(this);
        CreateSlots();

        inventoryPanel.SetActive(false);
        detailPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        inventory.RemoveObserver(this);
    }

    public void OpenUI()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);

        UpdateSlots();
        UpdateDetail();
    }

    public void CloseUI()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    private void CreateSlots()
    {
        WorkTableItem[] itemList = inventory.ItemList;

        for (int i = 0; i < itemList.Length; i++)
        {
            Transform parent = GetParentByLevel(itemList[i].requiredLevel);

            WorkTableItemSlot slot = Instantiate(slotPrefab, parent);
            slot.SetUp(itemList[i], inventory, this);

            slots.Add(slot);
        }
    }

    private Transform GetParentByLevel(int level)
    {
        if (level == 1) return level1Parent;
        if (level == 2) return level2Parent;
        if (level == 3) return level3Parent;

        return level1Parent;
    }

    private void UpdateSlots()
    {
        foreach (WorkTableItemSlot slot in slots)
        {
            slot.UpdateSlot();
        }
    }

    public void SelectSlot(WorkTableItemSlot slot, WorkTableItem item)
    {
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        selectedSlot = slot;
        selectedItem = item;
        selectedSlot.SetSelected(true);

        UpdateDetail();
    }

    public void BuySelectedItem()
    {
        if (selectedItem == null) return;

        if (!inventory.IsUnlocked(selectedItem))
        {
            Debug.Log($"{selectedItem.itemName} 구매할 수 없습니다.");
            return;
        }

        if (inventory.IsSoldOut(selectedItem))
        {
            Debug.Log($"{selectedItem.itemName} 구매 완료된 아이템입니다.");
            return;
        }

        if (!inventory.HasMaterial(selectedItem))
        {
            Debug.Log($"{selectedItem.itemName} 구매 재료가 부족합니다.");
            return;
        }

        if (!inventory.BuyItem(selectedItem))
        {
            Debug.Log($"{selectedItem.itemName} 구매에 실패했습니다.");
            return;
        }

        Debug.Log($"{selectedItem.itemName} 구매 완료");

        UpdateSlots();
        UpdateDetail();
    }

    private void UpdateDetail()
    {
        bool hasItem = selectedItem != null;

        detailPanel.SetActive(hasItem);
        if (!hasItem) return;

        SetDetailInfo(selectedItem);
        SetDetailCost(selectedItem);
        SetCraftButton(selectedItem);
    }

    private void SetCraftButton(WorkTableItem item)
    {
        bool unlocked = inventory.IsUnlocked(item);
        bool soldOut = inventory.IsSoldOut(item);

        if (!unlocked)
        {
            SetCraftStatus("잠겨 있음");
        }
        else if (soldOut)
        {
            SetCraftStatus("구매 완료");
        }
        else
        {
            SetCraftAvailable();
        }
    }

    private void SetCraftStatus(string text)
    {
        craftButton.gameObject.SetActive(false);
        craftText.gameObject.SetActive(true);
        craftText.text = text;
    }

    private void SetCraftAvailable()
    {
        craftButton.gameObject.SetActive(true);
        craftText.gameObject.SetActive(false);
    }

    private void SetDetailInfo(WorkTableItem item)
    {
        iconImage.sprite = item.iconImage;
        nameText.text = item.itemName;
        descriptionText.text = item.description;
    }

    private void SetDetailCost(WorkTableItem item)
    {
        ironCost.SetActive(item.needIron > 0);
        woodCost.SetActive(item.needWood > 0);

        ironCostText.text = item.needIron.ToString();
        woodCostText.text = item.needWood.ToString();

        ironCostText.color = inventory.Iron < item.needIron ? lackMaterialColor : Color.white;
        woodCostText.color = inventory.Wood < item.needWood ? lackMaterialColor : Color.white;
    }

    public void Notify()
    {
        UpdateSlots();
        UpdateDetail();
    }
}
