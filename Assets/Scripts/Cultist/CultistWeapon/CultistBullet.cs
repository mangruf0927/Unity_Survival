using UnityEngine;

public class CultistBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rigid;
    private int damage;
    private float speed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
    }

    public void SetData(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
    }

    public void Fire(Vector3 direction)
    {
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.linearVelocity = direction.normalized * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player") && other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.transform.root.CompareTag("Player")) return;

        Destroy(gameObject);
    }
}
