using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Image barFill;

    [SerializeField] private GameObject visualsRoot;

    [Header("Behavior")]
    [SerializeField] private bool alwaysShow = false;
    [SerializeField] private bool startHidden = true;
    [SerializeField] private float hideDelay = 2f;

    [Header("Optional Rules")]
    [SerializeField] private bool keepVisibleWhenLow = true;
    [SerializeField, Range(0f, 1f)] private float lowThreshold = 0.25f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        // visualsRoot 자동 할당 (안 꽂았으면 barFill의 부모를 사용)
        if (visualsRoot == null && barFill != null)
            visualsRoot = barFill.transform.parent.gameObject;

        // 시작 상태
        if (alwaysShow) Show();
        else
        {
            if (startHidden) HideImmediate();
            else Show();
        }
    }

    public void SetNormalized(float t)
    {
        if (barFill == null) return;

        t = Mathf.Clamp01(t);
        barFill.fillAmount = t;

        if (alwaysShow)
        {
            Show();
            return;
        }

        // 너무 낮으면 계속 보여주기
        if (keepVisibleWhenLow && t <= lowThreshold)
        {
            Show();
            StopHideRoutine();
            return;
        }

        // 기본: 갱신될 때만 잠깐 보였다가 숨김
        Show();
        RestartHideTimer();
    }

    public void Show()
    {
        if (visualsRoot != null && !visualsRoot.activeSelf)
            visualsRoot.SetActive(true);
    }

    public void HideImmediate()
    {
        StopHideRoutine();
        if (visualsRoot != null && visualsRoot.activeSelf)
            visualsRoot.SetActive(false);
    }

    private void RestartHideTimer()
    {
        StopHideRoutine();
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        if (visualsRoot != null) visualsRoot.SetActive(false);
        hideRoutine = null;
    }

    private void StopHideRoutine()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }
    }
}
