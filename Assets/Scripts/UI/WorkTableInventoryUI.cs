using UnityEngine;

public class WorkTableInventoryUI : MonoBehaviour
{
    [SerializeField] private WorkTableInventory inventory;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Slot")]
    [SerializeField] private WorkTableItemSlot slotPrefab;
    [SerializeField] private Transform slotParent;

    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        CreateSlots();
        inventoryPanel.SetActive(false);
    }

    public void OpenUI()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
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
            WorkTableItemSlot slot = Instantiate(slotPrefab, slotParent);
            slot.SetUp(itemList[i], inventory);
        }
    }
}