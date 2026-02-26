using Unity.Burst.Intrinsics;
using UnityEngine;

public enum WeaponTypeEnums { MELEE, RANGED }

public abstract class Weapon : MonoBehaviour
{
    public WeaponTypeEnums weaponType;

    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new Collider collider;

    public bool canDrop;
    public Vector3 aimPos;
    public void SetAimPoint(Vector3 pos) {aimPos = pos;}

    public virtual void Pick(Transform position)
    {
        transform.SetParent(position);
        transform.SetLocalPositionAndRotation(new Vector3(0.2f, -1f, 0.5f), Quaternion.Euler(0f, 0f, -90f));        
        
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
