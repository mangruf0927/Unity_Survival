using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemId;

    private ItemData itemData;
    public ItemData Data => itemData;

    private void Awake()
    {
        ItemData data = DataManager.Instance.ItemTable.Get(itemId);
        SetUp(data);
    }

    public void SetUp(ItemData data)
    {
        itemData = data;
    }
}
