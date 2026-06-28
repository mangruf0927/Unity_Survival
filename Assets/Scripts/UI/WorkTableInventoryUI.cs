using System.Collections.Generic;
using UnityEngine;

public class WorkTableInventoryUI : MonoBehaviour, IObserver
{
    [SerializeField] private WorkTableInventory inventory;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Slot")]
    [SerializeField] private WorkTableItemSlot slotPrefab;
    [SerializeField] private Transform level1Parent;
    [SerializeField] private Transform level2Parent;

    private readonly List<WorkTableItemSlot> slots = new();

    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        inventory.AddObserver(this);
        CreateSlots();
        inventoryPanel.SetActive(false);
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
            slot.SetUp(itemList[i], inventory);
            slots.Add(slot);
        }
    }

    private Transform GetParentByLevel(int level)
    {
        if (level == 1) return level1Parent;
        if (level == 2) return level2Parent;

        return level1Parent;
    }

    private void UpdateSlots()
    {
        foreach (WorkTableItemSlot slot in slots)
        {
            slot.UpdateSlot();
        }
    }

    public void Notify()
    {
        UpdateSlots();
    }
}