using System.Collections.Generic;
using UnityEngine;

public class CultistHPBarController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private readonly Dictionary<CultistStats, CultistHPBar> hpBarDictionary = new();

    private void Awake()
    {
        mainCamera = Camera.main;
        if (canvas != null) canvasRect = canvas.transform as RectTransform;
    }

    private void LateUpdate()
    {
        foreach (var dic in hpBarDictionary)
        {
            CultistStats cultistStats = dic.Key;
            CultistHPBar hpBar = dic.Value;

            if (cultistStats == null || hpBar == null || cultistStats.HPBarPoint == null) continue;
            UpdatePosition(hpBar.transform as RectTransform, cultistStats.HPBarPoint.position);
        }
    }

    private void UpdatePosition(RectTransform rectTransform, Vector3 worldPos)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPos);
        bool isInvisible = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;
        if (isInvisible)
        {
            if (rectTransform.gameObject.activeSelf)
            {
                rectTransform.gameObject.SetActive(false);
            }

            return;
        }

        if (!rectTransform.gameObject.activeSelf)
        {
            rectTransform.gameObject.SetActive(true);
        }

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos))
        {
            rectTransform.localPosition = localPos;
        }
    }

    public void Register(CultistStats cultistStats)
    {
        if (cultistStats == null) return;

        cultistStats.OnDamaged += ShowHPBar;
        cultistStats.OnDead += HideHPBar;
    }

    public void UnRegister(CultistStats cultistStats)
    {
        if (cultistStats == null) return;

        cultistStats.OnDamaged -= ShowHPBar;
        cultistStats.OnDead -= HideHPBar;

        HideHPBar(cultistStats);
    }

    public void ShowHPBar(CultistStats cultistStats)
    {
        if (cultistStats == null) return;

        if (hpBarDictionary.TryGetValue(cultistStats, out CultistHPBar hpBar))
        {
            hpBar.UpdateHPBar();
            return;
        }

        GameObject hpObject = ObjectPool.Instance.GetFromPool(PoolTypeEnums.HPBAR);
        if (hpObject == null) return;

        CultistHPBar cultistHPBar = hpObject.GetComponent<CultistHPBar>();
        if (cultistHPBar == null) return;

        cultistHPBar.SetHPBar(cultistStats);

        hpBarDictionary.Add(cultistStats, cultistHPBar);
    }

    public void HideHPBar(CultistStats cultistStats)
    {
        if (!hpBarDictionary.TryGetValue(cultistStats, out CultistHPBar hpBar)) return;
        hpBarDictionary.Remove(cultistStats);

        if (hpBar == null || !hpBar.gameObject.activeSelf) return;
        hpBar.Clear();
        ObjectPool.Instance.ReturnToPool(hpBar.gameObject, PoolTypeEnums.HPBAR);
    }
}
