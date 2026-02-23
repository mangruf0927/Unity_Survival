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
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public abstract void EnterAttack();
    public abstract void Attack();
    public abstract void ExitAttack();
}
