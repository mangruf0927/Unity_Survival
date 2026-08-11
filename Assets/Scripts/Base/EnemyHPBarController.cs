using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private RectTransform canvasRect;

    private readonly Dictionary<EnemyStatsBase, EnemyHPBar> hpBarDictionary = new();

    private void Awake()
    {
        mainCamera = Camera.main;

        if (canvas != null)
            canvasRect = canvas.transform as RectTransform;
    }

    private void LateUpdate()
    {
        foreach (var dic in hpBarDictionary)
        {
            EnemyStatsBase enemyStats = dic.Key;
            EnemyHPBar hpBar = dic.Value;

            if (enemyStats == null || hpBar == null || enemyStats.HPBarPoint == null) continue;
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
            if (rectTransform.gameObject.activeSelf)        // 상태가 바뀔 때만 호출
            {
                rectTransform.gameObject.SetActive(false);
            }

            return;
        }

        if (!rectTransform.gameObject.activeSelf)           // 상태가 바뀔 때만 호출
        {
            rectTransform.gameObject.SetActive(true);
        }

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            rectTransform.localPosition = localPos;
        }
    }

    public void Register(EnemyStatsBase stats)
    {
        if (stats == null)
            return;

        stats.OnDamaged -= ShowHPBar;
        stats.OnDead -= HideHPBar;

        stats.OnDamaged += ShowHPBar;
        stats.OnDead += HideHPBar;
    }

    public void UnRegister(EnemyStatsBase stats)
    {
        if (stats == null)
            return;

        stats.OnDamaged -= ShowHPBar;
        stats.OnDead -= HideHPBar;

        HideHPBar(stats);
    }

    private void ShowHPBar(EnemyStatsBase stats)
    {
        if (stats == null) return;
        if (hpBarDictionary.TryGetValue(stats, out EnemyHPBar hpBar))
        {
            hpBar.UpdateHPBar();
            return;
        }

        GameObject hpObject = ObjectPool.Instance.GetFromPool(PoolTypeEnums.HPBAR);

        if (hpObject == null) return;

        EnemyHPBar enemyHPBar = hpObject.GetComponent<EnemyHPBar>();

        if (enemyHPBar == null)
        {
            ObjectPool.Instance.ReturnToPool(hpObject, PoolTypeEnums.HPBAR);
            return;
        }

        enemyHPBar.SetHPBar(stats);

        hpBarDictionary.Add(stats, enemyHPBar);
    }

    private void HideHPBar(EnemyStatsBase stats)
    {
        if (stats == null) return;
        if (!hpBarDictionary.TryGetValue(stats, out EnemyHPBar hpBar)) return;

        hpBarDictionary.Remove(stats);

        if (hpBar == null) return;

        hpBar.Clear();

        ObjectPool.Instance.ReturnToPool(hpBar.gameObject, PoolTypeEnums.HPBAR);
    }
}
