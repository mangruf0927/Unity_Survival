using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemId;

    private ItemRegistry itemRegistry;

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

    public void SetItemSpawner(ItemRegistry registry)
    {
        itemRegistry = registry;
    }

    public void UnregisterItem()
    {
        if (itemRegistry == null) return;

        itemRegistry.Unregister(this);
        itemRegistry = null;
    }

    public void ResetPhysics()
    {
        if (rigid == null) return;

        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
}
