using UnityEngine;

public class PlayerStat : MonoBehaviour, IDamageable
{
    [SerializeField] PlayerStateMachine playerStateMachine;

    [Header("이동 속도")]
    public float moveSpeed;  

    [Header("달리기 속도")]
    public float runSpeed;  

    [Header("점프 높이")]
    public float jumpForce;

    [Header("회전 속도")]
    public float rotateSpeed;

    [Header("체력")]
    [SerializeField] private int maxHP = 100;
    
    public int CurrentHP { get; private set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if(dmg <= 0) return;

        CurrentHP = Mathf.Max(CurrentHP - dmg, 0);
        if(CurrentHP <= 0) playerStateMachine.ChangeState(PlayerStateEnums.DEAD);
    }
}
