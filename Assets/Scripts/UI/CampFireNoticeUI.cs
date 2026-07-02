using System.Collections;
using TMPro;
using UnityEngine;

public class CampFireNoticeUI : MonoBehaviour
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private float noticeTime = 3f;

    private Coroutine noticeCoroutine;

    private void Start()
    {
        if (campFire == null) return;

        campFire.OnNotice += ShowNotice;
        noticeText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (campFire == null) return;

        campFire.OnNotice -= ShowNotice;
    }

    private void ShowNotice(CampFireNoticeType noticeType)
    {
        if (noticeCoroutine != null)
        {
            StopCoroutine(noticeCoroutine);
        }

        noticeCoroutine = StartCoroutine(ShowNoticeRoutine(GetNoticeMessage(noticeType)));
    }

    private IEnumerator ShowNoticeRoutine(string message)
    {
        noticeText.text = message;
        noticeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(noticeTime);

        noticeText.gameObject.SetActive(false);
        noticeCoroutine = null;
    }

    private string GetNoticeMessage(CampFireNoticeType noticeType)
    {
        return noticeType switch
        {
            CampFireNoticeType.WARNING => "캠프파이어가 거의 꺼졌어요",
            CampFireNoticeType.EXTINGUISHED => "불이 꺼졌습니다.\n 더 이상 안전하지 않습니다",
            _ => string.Empty
        };
    }
}
