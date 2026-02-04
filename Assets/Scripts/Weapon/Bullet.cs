using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;
    
    private float lifeTime = 3;
    private int attackDamage;

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

    void OnTriggerEnter(Collider other)
    {
        if (attackDamage <= 0) return;
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<IDamageable>();
        enemy.TakeDamage(attackDamage);
    }
}
