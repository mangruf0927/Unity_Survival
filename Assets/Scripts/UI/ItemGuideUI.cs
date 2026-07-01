using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGuideUI : MonoBehaviour
{
    [SerializeField] private List<Image> slotList;

    [SerializeField] private Sprite Ekey;
    [SerializeField] private Sprite FKey;
    [SerializeField] private Sprite RKey;
    [SerializeField] private Sprite BackKey;

    public void UpdateUI(EquippableItem equippedItem, Item hoveredItem, EquippableItem hoveredEquippable)
    {
        ClearUI();

        List<Sprite> showList = new();
        Sack sack = equippedItem as Sack;

        if (hoveredEquippable != null)
        {
            showList.Add(Ekey);
        }
        else if (hoveredItem != null)
        {
            ItemType itemType = hoveredItem.Data.ItemType;

            if (itemType == ItemType.FOOD || itemType == ItemType.AMMO)
            {
                showList.Add(Ekey);
            }

            if (sack != null && !sack.IsFull)
            {
                showList.Add(FKey);
            }
        }
        else if (sack != null)
        {
            if (!sack.IsEmpty)
            {
                showList.Add(FKey);
            }
        }
        else if (equippedItem != null)
        {
            if (equippedItem is RangedWeapon)
            {
                showList.Add(RKey);
            }

            if (equippedItem.CanDrop)
            {
                showList.Add(BackKey);
            }
        }

        ShowImages(showList);
    }

    private void ShowImages(List<Sprite> showList)
    {
        for (int i = 0; i < showList.Count && i < slotList.Count; i++)
        {
            slotList[i].sprite = showList[i];
            slotList[i].enabled = true;
        }
    }

    private void ClearUI()
    {
        foreach (Image slot in slotList)
        {
            slot.sprite = null;
            slot.enabled = false;
        }
    }
}
