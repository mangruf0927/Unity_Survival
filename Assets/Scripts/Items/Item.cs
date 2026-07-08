using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemId;

    private ItemSpawner itemSpawner;

    private ItemData itemData;
    private Rigidbody rigid;

    public ItemData Data => itemData;
    public int ItemId => itemId;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        ItemData data = DataManager.Instance.ItemTable.Get(itemId);
        SetUp(data);
    }

    private void OnDestroy()
    {
        UnregisterItem();
    }

    public void SetUp(ItemData data)
    {
        itemData = data;
    }

    public void SetItemSpawner(ItemSpawner spawner)
    {
        itemSpawner = spawner;
    }

    public void UnregisterItem()
    {
        if (itemSpawner == null) return;

        itemSpawner.Unregister(this);
        itemSpawner = null;
    }

    public void ResetPhysics()
    {
        if (rigid == null) return;

        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
}
