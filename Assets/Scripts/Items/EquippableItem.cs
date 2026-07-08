using UnityEngine;

public abstract class EquippableItem : MonoBehaviour
{
    [SerializeField] private ToolType itemType;
    [SerializeField] private Transform attachPoint;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private int itemId;

    private EquippableSpawner equippableSpawner;
    protected string itemName;

    private Rigidbody rigid;
    private Collider[] colliders;

    public ToolType ItemType => itemType;
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public int ItemId => itemId;
    public bool IsAttached { get; private set; }

    protected bool canDrop = true;
    public virtual bool CanDrop => canDrop;

    private void Awake()
    {
        InitComponents();
    }

    public void SetEquippableSpawner(EquippableSpawner spawner)
    {
        equippableSpawner = spawner;
    }

    public void UnregisterEquippable()
    {
        if (equippableSpawner == null) return;

        equippableSpawner.Unregister(this);
        equippableSpawner = null;
    }

    private void OnDestroy()
    {
        UnregisterEquippable();
    }

    public virtual void Attach(Transform position)
    {
        if (position == null)
        {
            Debug.LogError($"{name} attach failed. Equip position is missing.", this);
            return;
        }

        InitComponents();
        IsAttached = true;

        if (attachPoint != null)
        {
            transform.rotation = position.rotation * Quaternion.Inverse(attachPoint.rotation) * transform.rotation;
            transform.position += position.position - attachPoint.position;
        }
        else
        {
            Debug.LogError($"{name} attach point is missing. Using item transform as the attach point.", this);
            transform.SetPositionAndRotation(position.position, position.rotation);
        }

        transform.SetParent(position, true);

        foreach (Collider col in colliders)
        {
            if (col != null)
            {
                col.enabled = false;
                col.isTrigger = true;
            }
        }

        if (rigid != null)
        {
            rigid.isKinematic = true;
            rigid.useGravity = false;
        }
    }

    public virtual void Detach()
    {
        InitComponents();
        IsAttached = false;

        transform.SetParent(null);

        foreach (Collider col in colliders)
        {
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = false;
            }
        }

        if (rigid != null)
        {
            rigid.isKinematic = false;
            rigid.useGravity = true;
        }
    }

    private void InitComponents()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody>();
        if (colliders == null || colliders.Length == 0) colliders = GetComponents<Collider>();
    }

    public abstract void OnEquip(PlayerController player);
    public abstract void OnUnequip(PlayerController player);
}
