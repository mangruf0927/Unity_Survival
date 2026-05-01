using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemId;

    private ItemDataTable itemData;
    public ItemDataTable Data => itemData;

    private IEnumerator Start()
    {
        if (!DataManager.Instance.IsLoaded)
        {
            yield return DataManager.Instance.LoadAll();
        }

        ItemDataTable data = DataManager.Instance.ItemTable.Get(itemId);
        SetUp(data);
    }

    public void SetUp(ItemDataTable data)
    {
        itemData = data;
    }
}
