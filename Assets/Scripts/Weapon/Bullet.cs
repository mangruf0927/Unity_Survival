using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;

    private ObjectPool pool;
    private int damage;
    private float speed;
    private readonly float lifeTime = 10;

    void Awake()
    {
        rigid.useGravity = false;
        rigid.isKinematic = false;
    }
    
    public void SetData(int damage, float speed, ObjectPool pool)
    {
        this.damage = damage;
        this.speed = speed;
        this.pool = pool;
    }

    public void Fire(Vector3 direction)
    {
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        
        rigid.linearVelocity = direction.normalized * speed;
        Invoke(nameof(DisableBullet), lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player")) return;

        if (other.CompareTag("Enemy") && other.TryGetComponent<IDamageable>(out var enemy))
            enemy.TakeDamage(damage);

        DisableBullet();
    }

    private void DisableBullet()
    {
        pool.ReturnToPool(gameObject, PoolTypeEnums.BULLET);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
