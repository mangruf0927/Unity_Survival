using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemId;

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

    public void SetUp(ItemData data)
    {
        itemData = data;
    }

    public void ResetPhysics()
    {
        if (rigid == null) return;

        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
}
