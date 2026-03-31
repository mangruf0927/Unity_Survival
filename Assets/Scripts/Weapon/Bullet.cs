using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rigid;
    private int damage;
    private float speed;

    private readonly float lifeTime = 10;

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
        ObjectPool.Instance.ReturnToPool(gameObject, PoolTypeEnums.BULLET);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
