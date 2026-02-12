using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;

    private int damage;
    private float speed;
    private readonly float lifeTime = 10;

    void Awake()
    {
        rigid.useGravity = false;
        rigid.isKinematic = false;
    }
    
    public void SetData(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
    }

    public void Fire(Vector3 direction)
    {
        rigid.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player")) return;

        if (other.CompareTag("Enemy") && other.TryGetComponent<IDamageable>(out var enemy))
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
