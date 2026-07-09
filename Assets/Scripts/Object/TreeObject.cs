using System.Collections;
using UnityEngine;

public class TreeObject : WorldObject
{
    [SerializeField] private int logItemId;
    [SerializeField] private ItemRegistry itemRegistry;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private float shakeAngle;
    [SerializeField] private float shakeTime;

    private int currentHp;
    private Quaternion original;
    private bool isShaking;

    private void Awake()
    {
        currentHp = maxHP;
        original = transform.rotation;
    }

    public override ObjectSaveData CreateSaveData()
    {
        ObjectSaveData data = base.CreateSaveData();

        data.treeSaveData = new TreeSaveData
        {
            currentHp = currentHp
        };

        return data;
    }

    public override void LoadSaveData(ObjectSaveData data)
    {
        base.LoadSaveData(data);

        if (data.treeSaveData == null) return;
        currentHp = data.treeSaveData.currentHp;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHp <= 0) return;

        currentHp = Mathf.Max(currentHp - damage, 0);

        if (!isShaking) StartCoroutine(Shake());
        if (currentHp == 0)
        {
            StopAllCoroutines();
            CreateLog();
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;

        Quaternion left = original * Quaternion.Euler(0f, 0f, shakeAngle);
        Quaternion right = original * Quaternion.Euler(0f, 0f, -shakeAngle);

        yield return Rotate(original, left, shakeTime * 0.33f);
        yield return Rotate(left, right, shakeTime * 0.34f);
        yield return Rotate(right, original, shakeTime * 0.33f);

        transform.rotation = original;
        isShaking = false;
    }

    private IEnumerator Rotate(Quaternion from, Quaternion to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(from, to, time / duration);
            yield return null;
        }
    }

    private void CreateLog()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 randomPos = new(Random.Range(-0.5f, 0.5f), 0.3f, Random.Range(-0.5f, 0.5f));
            itemRegistry.SpawnItem(logItemId, transform.position + randomPos, Quaternion.identity);
        }
    }
}

