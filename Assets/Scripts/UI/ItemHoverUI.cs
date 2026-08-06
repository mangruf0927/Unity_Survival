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

    private Transform targetTransform;
    private Collider targetCollider;

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
        if (targetTransform == null) return;
        UpdatePosition();
    }

    public void ShowUI(Item item)
    {
        if (item == null)
        {
            HideUI();
            return;
        }
        SetTarget(item.transform, item.GetComponentInChildren<Collider>());

        HoverUI.SetActive(true);
        HideImages();

        int count = Mathf.Min(item.Data.Value, imageList.Count);

        if (imageDictionary.TryGetValue(item.Data.ItemType, out Sprite targetSprite))
        {
            for (int i = 0; i < count; i++)
            {
                imageList[i].gameObject.SetActive(true);
                imageList[i].sprite = targetSprite;
            }
        }

        itemName.text = item.Data.Name;
        UpdatePosition();
    }

    public void ShowUI(EquippableItem item)
    {
        if (item == null)
        {
            HideUI();
            return;
        }

        SetTarget(item.transform, item.GetComponentInChildren<Collider>());

        HoverUI.SetActive(true);
        HideImages();

        itemName.text = item.ItemName;
        UpdatePosition();
    }

    private void SetTarget(Transform target, Collider collider)
    {
        targetTransform = target;
        targetCollider = collider;
    }

    private void UpdatePosition()
    {
        Vector3 itemPosition = targetCollider != null ? targetCollider.bounds.center : targetTransform.position;
        Vector3 worldPos = itemPosition + offset;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            rectTransform.localPosition = localPos;
        }
    }

    public void HideUI()
    {
        targetTransform = null;
        targetCollider = null;

        itemName.text = "";

        HideImages();
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
