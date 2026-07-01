using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGuideUI : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> guideGroupList;
    [SerializeField] private List<Image> slotList;
    [SerializeField] private List<TextMeshProUGUI> textList;

    [SerializeField] private Sprite Ekey;
    [SerializeField] private Sprite FKey;
    [SerializeField] private Sprite RKey;
    [SerializeField] private Sprite BackKey;

    private readonly struct GuideInfo
    {
        public readonly Sprite KeySprite;
        public readonly string Text;

        public GuideInfo(Sprite keySprite, string text)
        {
            KeySprite = keySprite;
            Text = text;
        }
    }

    public void UpdateUI(EquippableItem equippedItem, Item hoveredItem, EquippableItem hoveredEquippable)
    {
        ClearUI();

        List<GuideInfo> showList = new();
        Sack sack = equippedItem as Sack;

        if (hoveredEquippable != null)
        {
            showList.Add(new GuideInfo(Ekey, "가져가기"));
        }
        else if (hoveredItem != null)
        {
            ItemType itemType = hoveredItem.Data.ItemType;

            if (itemType == ItemType.FOOD)
            {
                showList.Add(new GuideInfo(Ekey, "먹기"));
            }
            else if (itemType == ItemType.AMMO)
            {
                showList.Add(new GuideInfo(Ekey, "가져가기"));
            }

            if (sack != null && !sack.IsFull)
            {
                showList.Add(new GuideInfo(FKey, "상점"));
            }
        }
        else if (sack != null)
        {
            if (!sack.IsEmpty)
            {
                showList.Add(new GuideInfo(FKey, "저장 해제"));
            }
        }
        else if (equippedItem != null)
        {
            Weapon weapon = equippedItem as Weapon;

            if (weapon is RangedWeapon)
            {
                showList.Add(new GuideInfo(RKey, "재장전"));
            }

            if (weapon != null && weapon.CanDrop)
            {
                showList.Add(new GuideInfo(BackKey, "드롭"));
            }
        }

        ShowGuides(showList);
    }

    private void ShowGuides(List<GuideInfo> showList)
    {
        for (int i = 0; i < showList.Count && i < slotList.Count; i++)
        {
            slotList[i].sprite = showList[i].KeySprite;

            if (i < textList.Count)
            {
                textList[i].text = showList[i].Text;
            }

            if (i < guideGroupList.Count)
            {
                SetVisible(guideGroupList[i], true);
            }
        }
    }

    private void ClearUI()
    {
        foreach (Image slot in slotList)
        {
            slot.sprite = null;
        }

        foreach (TextMeshProUGUI text in textList)
        {
            text.text = "";
        }

        foreach (CanvasGroup guideGroup in guideGroupList)
        {
            SetVisible(guideGroup, false);
        }
    }

    private void SetVisible(CanvasGroup guideGroup, bool visible)
    {
        guideGroup.alpha = visible ? 1f : 0f;
        guideGroup.blocksRaycasts = visible;
        guideGroup.interactable = visible;
    }
}
