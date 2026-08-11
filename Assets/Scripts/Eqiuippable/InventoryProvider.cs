using System.Collections.Generic;
using UnityEngine;

public class InventoryProvider : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> basicItemList = new();

    private Inventory inventory;
    public Inventory Inventory => inventory;

    private void Awake()
    {
        inventory = new Inventory(basicItemList);
    }
}
