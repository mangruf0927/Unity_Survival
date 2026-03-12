using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float shakeAngle;
    [SerializeField] private float shakeTime;

    private int currentHP;
    private Quaternion original;
    private bool isShaking;

    private void Awake()
    {
        currentHP = maxHP;
        original = transform.rotation;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHP <= 0) return;

        currentHP = Mathf.Max(currentHP - damage, 0);

        if (!isShaking) StartCoroutine(Shake());
        if (currentHP == 0) Destroy(gameObject);
    }

    private IEnumerator Shake()
    {
        isShaking = true;

        Quaternion left = original * Quaternion.Euler(0f, 0f, shakeAngle);
        Quaternion right = original * Quaternion.Euler(0f, 0f, -shakeAngle);

        yield return Rotate(original, left, shakeTime * 0.5f);
        yield return Rotate(left, right, shakeTime * 0.5f);

        transform.rotation = original;
        isShaking = false;
    }

    private IEnumerator Rotate(Quaternion from, Quaternion to, float duration)
    {
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(from, to, time / duration);
            yield return null;
        }
    }
}

