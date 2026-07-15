using System.Collections;
using UnityEngine;

public class TreeObject : WorldObject
{
    [SerializeField] private int logItemId;
    [SerializeField] private ItemRegistry itemRegistry;

    [SerializeField] private int maxHP = 100;
    [SerializeField] private int logCount = 3;
    [SerializeField] private float shakeAngle;
    [SerializeField] private float shakeTime;

    private int currentHp;
    private bool isShaking;

    private void Awake()
    {
        currentHp = maxHP;
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
            Vector3 dropPosition = transform.position;
            gameObject.SetActive(false);
            CreateLog(dropPosition);
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;

        Vector3 originalPos = transform.position;

        yield return Move(originalPos, originalPos + Vector3.down * 0.15f, shakeTime * 0.5f);
        yield return Move(originalPos + Vector3.down * 0.15f, originalPos, shakeTime * 0.5f);

        isShaking = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, time / duration);
            yield return null;
        }
    }

    private void CreateLog(Vector3 center)
    {
        const float dropRadius = 0.7f;
        const float spawnHeight = 2f;
        const float randomRadius = 0.1f;

        for (int i = 0; i < logCount; i++)
        {
            float angle = i * Mathf.PI * 2f / logCount;

            Vector3 direction = new(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector2 randomCircle = Random.insideUnitCircle * randomRadius;
            Vector3 randomOffset = new(randomCircle.x, 0f, randomCircle.y);

            Vector3 spawnPosition = center + direction * dropRadius + randomOffset + Vector3.up * spawnHeight;
            Quaternion rotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

            itemRegistry.SpawnItem(logItemId, spawnPosition, rotation);
        }
    }
}

