using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private readonly Dictionary<EnemyStats, EnemyHPBar> hpBarDictionary = new();

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasRect = canvas.transform as RectTransform;
    }

    private void LateUpdate()
    {
        foreach (var dic in hpBarDictionary)
        {
            EnemyStats enemyStats = dic.Key;
            EnemyHPBar hpBar = dic.Value;

            UpdatePosition(hpBar.transform as RectTransform, enemyStats.HPBarPoint.position);
        }
    }

    private void UpdatePosition(RectTransform rectTransform, Vector3 worldPos)
    {
        // 카메라 밖으로 나갔을 때
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPos);
        bool isInvisible = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;
        if (isInvisible)
        {
            rectTransform.gameObject.SetActive(false);
            return;
        }
        rectTransform.gameObject.SetActive(true);

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            rectTransform.localPosition = localPos;
        }
    }

    public void Register(EnemyStats enemyStats)
    {
        enemyStats.OnDamaged += ShowHPBar;
        enemyStats.OnDead += HideHPBar;
    }

    public void UnRegister(EnemyStats enemyStats)
    {
        enemyStats.OnDamaged -= ShowHPBar;
        enemyStats.OnDead -= HideHPBar;

        HideHPBar(enemyStats);
    }

    public void ShowHPBar(EnemyStats enemyStats)
    {
        if (hpBarDictionary.TryGetValue(enemyStats, out EnemyHPBar hpBar))
        {
            hpBar.UpdateHPBar();
            return;
        }

        GameObject hpObject = ObjectPool.Instance.GetFromPool(PoolTypeEnums.HPBAR);
        EnemyHPBar enemyHPBar = hpObject.GetComponent<EnemyHPBar>();
        enemyHPBar.SetHPBar(enemyStats);

        hpBarDictionary.Add(enemyStats, enemyHPBar);
    }

    public void HideHPBar(EnemyStats enemyStats)
    {
        if (!hpBarDictionary.TryGetValue(enemyStats, out EnemyHPBar hpBar)) return;

        hpBar.Clear();
        hpBarDictionary.Remove(enemyStats);
        ObjectPool.Instance.ReturnToPool(hpBar.gameObject, PoolTypeEnums.HPBAR);
    }
}
