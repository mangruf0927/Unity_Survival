using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<InventorySlot> slotList;

    public delegate void SelectSlotHandler(int idx);
    public event SelectSlotHandler OnSelectSlot;

    private void Start()
    {
        inventory.OnChanged += UpdateSlot;

        for (int i = 0; i < slotList.Count; i++)
            slotList[i].OnClickSlot += ClickSlot;

        UpdateSlot();
    }

    private void OnDestroy()
    {
        inventory.OnChanged -= UpdateSlot;

        for (int i = 0; i < slotList.Count; i++)
            slotList[i].OnClickSlot -= ClickSlot;
    }

    public void UpdateSlot()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < inventory.ItemList.Count) slotList[i].SetSlot(inventory.ItemList[i], i);
            else slotList[i].HideSlot();
        }
    }

    public void ClickSlot(int idx)
    {
        OnSelectSlot?.Invoke(idx);
    }
}
