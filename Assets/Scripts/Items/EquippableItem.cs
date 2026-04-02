using UnityEngine;

public abstract class EquippableItem : MonoBehaviour
{
    [SerializeField] private ToolType itemType;
    [SerializeField] private Transform attachPoint;
    [SerializeField] private Sprite itemIcon;

    private Rigidbody rigid;
    private Collider[] colliders;

    public ToolType ItemType => itemType;
    public Sprite ItemIcon => itemIcon;
    public virtual bool CanDrop => true;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    public virtual void Attach(Transform position)
    {
        transform.rotation = position.rotation * Quaternion.Inverse(attachPoint.rotation) * transform.rotation;
        transform.position += position.position - attachPoint.position;
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

    public abstract void OnEquip(PlayerController player);
    public abstract void OnUnequip(PlayerController player);
}
