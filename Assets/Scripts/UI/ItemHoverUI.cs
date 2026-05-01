using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemHoverUI : MonoBehaviour
{
    [SerializeField] private GameObject HoverUI;

    [SerializeField] private List<Image> imageList;
    [SerializeField] private Sprite fuelImage;
    [SerializeField] private Sprite materialImage;
    [SerializeField] private Sprite foodImage;
    [SerializeField] private TextMeshProUGUI itemName;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rectTransform;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private Item currentItem;

    private readonly Dictionary<ItemType, Sprite> imageDictionary = new();
    private readonly Vector3 offset = new(0f, 1.5f, 0f);

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasRect = canvas.transform as RectTransform;

        imageDictionary.Add(ItemType.FUEL, fuelImage);
        imageDictionary.Add(ItemType.MATERIAL, materialImage);
        imageDictionary.Add(ItemType.FOOD, foodImage);

        HideUI();
    }

    private void Update()
    {
        if (currentItem == null) return;

        UpdatePosition();
    }

    public void ShowUI(Item item)
    {
        if (item == null)
        {
            HideUI();
            return;
        }

        currentItem = item;
        HoverUI.SetActive(true);

        HideImages();

        int cnt = Mathf.Min(currentItem.Data.Value, imageList.Count);

        if (imageDictionary.TryGetValue(currentItem.Data.ItemType, out Sprite targetSprite))
        {
            for (int i = 0; i < cnt; i++)
            {
                imageList[i].gameObject.SetActive(true);
                imageList[i].sprite = targetSprite;
            }
        }

        itemName.text = currentItem.Data.Name;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 worldPos = currentItem.transform.position + offset;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            rectTransform.localPosition = localPos;
        }
    }

    public void HideUI()
    {
        currentItem = null;
        itemName.text = "";

        foreach (Image image in imageList)
        {
            image.gameObject.SetActive(false);
        }

        HoverUI.SetActive(false);
    }

    private void HideImages()
    {
        foreach (Image image in imageList)
        {
            image.gameObject.SetActive(false);
        }
    }
}