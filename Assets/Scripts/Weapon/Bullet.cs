using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float lifeTime;

    void Awake()
    {
        rigid.useGravity = false;
        rigid.isKinematic = false;
    }

    public void Fire(Vector3 direction, float speed)
    {
        rigid.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifeTime);
    }
}
