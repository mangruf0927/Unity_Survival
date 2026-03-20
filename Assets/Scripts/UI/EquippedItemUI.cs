using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedItemUI : MonoBehaviour
{
    [SerializeField] private List<Image> slotList;

    [SerializeField] private Sprite FKey;
    [SerializeField] private Sprite RKey;
    [SerializeField] private Sprite BackKey;

    public void UpdateUI(EquippableItem equippableItem)
    {
        ClearUI();
        if(equippableItem == null) return;

        List<Sprite> showList = new();
        if(equippableItem is Sack) showList.Add(FKey);
        else
        {
            if(equippableItem is RangedWeapon) showList.Add(RKey);
            if(equippableItem.CanDrop) showList.Add(BackKey);
        }
        
        for(int i = 0; i < showList.Count; i++)
        {
            slotList[i].sprite = showList[i];
            slotList[i].enabled = true;
        }
    }

    private void ClearUI()
    {
        foreach(Image slot in slotList)
        {
            slot.sprite = null;
            slot.enabled = false;
        }
    }
}
