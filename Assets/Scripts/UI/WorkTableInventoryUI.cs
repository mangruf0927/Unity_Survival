using UnityEngine;
using UnityEngine.InputSystem;

public class WorkTableInventoryUI : MonoBehaviour
{
    [SerializeField] private WorkTableInventory inventory;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Slot")]
    [SerializeField] private WorkTableItemSlot slotPrefab;
    [SerializeField] private Transform slotParent;

    private bool isOpen;

    private void Awake()
    {
        CreateSlots();
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            ToggleUI();
        }
    }

    private void ToggleUI()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
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