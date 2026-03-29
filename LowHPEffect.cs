using Cinemachine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class LowHPEffect : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image BloodVignette;
    [SerializeField] private Image Heart;
    [SerializeField] private Image LowHPBloodEffect;

    [Header("Low HP Keep")]
    public float lowHpThreshold = 30f;
    [Range(0f, 1f)] public float lowHpAlpha = 0.7f;
    public float settleDuration = 0.2f;

    [Header("Hit Flash")]
    [Range(0f, 1f)] public float hitFlashAlpha = 1f;
    public float hitFlashIn = 0.03f;
    public float hitFlashOut = 0.18f;

    [Header("Camera Shake (Cinemachine Impulse)")]
    [SerializeField] private CinemachineImpulseSource impulse;
    [SerializeField] private float impulseForce = 1f;

    float baseAlpha = 0f;
    Coroutine running;
    bool flashing;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.onDamaged += OnDamaged;
        playerHealth.onHealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.onDamaged -= OnDamaged;
        playerHealth.onHealthChanged -= OnHealthChanged;
    }

    void OnHealthChanged(float currentHealth, float maxHealth)
    {
        baseAlpha = (currentHealth <= lowHpThreshold) ? lowHpAlpha : 0f;

        if (!flashing)
            FadeTo(baseAlpha, settleDuration);
    }

    void OnDamaged(float damage)
    {
        FlashHit();
        
        if (impulse != null)
            impulse.GenerateImpulseWithForce(impulseForce);
    }

    public void FlashHit()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(FlashRoutine());
    }

    public void FadeTo(float targetAlpha, float duration)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Fade(targetAlpha, duration));
    }

    IEnumerator FlashRoutine()
    {
        flashing = true;
        yield return Fade(hitFlashAlpha, hitFlashIn);
        yield return Fade(baseAlpha, hitFlashOut);
        flashing = false;
        running = null;
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        if (BloodVignette == null) yield break;
        if (Heart == null) yield break;
        if (LowHPBloodEffect == null) yield break;

        targetAlpha = Mathf.Clamp01(targetAlpha);

        Color c = BloodVignette.color;
        float startA = c.a;
        Color H = Heart.color;
        float startH = H.a;

        if (duration <= 0f)
        {
            BloodVignette.color = new Color(c.r, c.g, c.b, targetAlpha);
            Heart.color = new Color(1f, 1f, 1f, targetAlpha);
            LowHPBloodEffect.color = new Color(1f, 1f, 1f, targetAlpha);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float x = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(startA, targetAlpha, x);
            BloodVignette.color = new Color(c.r, c.g, c.b, a);
            Heart.color = new Color(1f, 1f, 1f, a);
            LowHPBloodEffect.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        BloodVignette.color = new Color(c.r, c.g, c.b, targetAlpha);
        Heart.color = new Color(1f, 1f, 1f, targetAlpha);
        LowHPBloodEffect.color = new Color(1f, 1f, 1f, targetAlpha);
    }
}
