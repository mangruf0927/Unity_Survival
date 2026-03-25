using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI slotNumber;

    public delegate void ClickSlotHandler(int idx);
    public event ClickSlotHandler OnClickSlot;

    private int slotIndex = -1;

    public void SetSlot(EquippableItem item, int idx)
    {
        slotIndex = idx;
        iconImage.sprite = item.ItemIcon;
        slotNumber.text = (idx + 1).ToString();
        gameObject.SetActive(true);
    }

    public void HideSlot()
    {
        slotIndex = -1;
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
