using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI slotNumber;
    [SerializeField] private TextMeshProUGUI count;

    public delegate void ClickSlotHandler(int idx);
    public event ClickSlotHandler OnClickSlot;

    private int slotIndex = -1;

    public void SetSlot(InventoryItem inventoryItem, int idx)
    {
        slotIndex = idx;
        iconImage.sprite = inventoryItem.Item.ItemIcon;
        slotNumber.text = (idx + 1).ToString();
        count.text = inventoryItem.Count >= 2
            ? inventoryItem.Count.ToString()
            : string.Empty;
        gameObject.SetActive(true);
    }

    public void HideSlot()
    {
        slotIndex = -1;
        iconImage.sprite = null;
        slotNumber.text = string.Empty;
        count.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (slotIndex == -1) return;

        OnClickSlot?.Invoke(slotIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotIndex == -1) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotIndex == -1) return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotIndex == -1) return;
    }
}
