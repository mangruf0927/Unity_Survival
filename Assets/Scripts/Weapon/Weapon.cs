using Unity.Burst.Intrinsics;
using UnityEngine;

public enum WeaponTypeEnums { MELEE, RANGED }

public abstract class Weapon : EquippableItem
{
    public WeaponTypeEnums weaponType;

    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;
    [SerializeField] private Transform attachPoint;

    public bool canDrop;
    public Vector3 aimPos;
    public override bool CanDrop => canDrop;

    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

    public override void OnEquip(PlayerController player)
    {
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        gameObject.SetActive(false);
    }

    public virtual void Pick(Transform position)
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

    public virtual void Drop()
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

    public abstract void EnterAttack();
    public abstract void Attack();
    public abstract void ExitAttack();
}
