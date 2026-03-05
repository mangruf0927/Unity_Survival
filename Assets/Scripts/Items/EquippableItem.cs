using System;
using UnityEngine;

public abstract class EquippableItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Transform attachPoint;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;

    public string ItemName => itemName;
    public virtual bool CanDrop => true;

    public virtual void Attach(Transform position)
    {
        transform.rotation = position.rotation * Quaternion.Inverse(attachPoint.rotation) * transform.rotation;
        transform.position += position.position - attachPoint.position;
        transform.SetParent(position, true);

        if (collider != null)
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }

        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
    }

    public virtual void Detach()
    {
        transform.SetParent(null);

        if (collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }

        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
    }

    public abstract void OnEquip(PlayerController player);
    public abstract void OnUnequip(PlayerController player);
}
