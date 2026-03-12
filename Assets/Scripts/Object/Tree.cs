using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;

    private int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
        Debug.Log("나무 : " + currentHP);
    }

    public void TakeDamage(int dmg)
    {
        if(dmg <= 0 || currentHP <= 0) return;
        currentHP = Mathf.Max(currentHP - dmg, 0);
        Debug.Log("나무 : " + currentHP);
        if(currentHP == 0) Destroy(gameObject);
    }
}
