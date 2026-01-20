using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;

    public void Stop()
    {
        rigid.linearVelocity = Vector3.zero;
    }
}
