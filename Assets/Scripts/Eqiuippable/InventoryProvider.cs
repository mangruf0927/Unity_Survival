using System.Collections.Generic;
using UnityEngine;

public class InventoryProvider : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> basicItemList = new();

    private Inventory inventory;
    public Inventory Inventory => inventory;

    private void Awake()
    {
        SetItemsActive(true);
        inventory = new Inventory(basicItemList);
    }

    private void Start()
    {
        SetItemsActive(false);
    }

    private void SetItemsActive(bool isActive)
    {
        foreach (InventoryItem basicItem in basicItemList)
        {
            if (basicItem == null || basicItem.Item == null) continue;

            basicItem.Item.gameObject.SetActive(isActive);
        }
    }
}
