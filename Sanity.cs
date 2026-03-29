using System;
using UnityEngine;

public class Sanity : MonoBehaviour
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float sanityDecreasePerSecond = 1f;
    [SerializeField] private float sanityDecreaseWhenCaught = 10f;
    [SerializeField] private bool isInSafeZone = true;
    [SerializeField] private float damageSanityCooldown = 3f;
    [SerializeField] private float sanityRegenPerSecondInSafeZone = 2f;
    [SerializeField] private Health health;

    private float lastDamageSanityTime = -999f;
    public float Max => max;
    public float Current { get; private set; }
    public float DecreaseWhenCaught => sanityDecreaseWhenCaught;

    public event Action<float, float> onSanityChanged;
    private void Awake()
    {
        Current = max;
        Notify();
    }
    private void Update()
    {
        if (!isInSafeZone)
        {
            DecreaseSanity(sanityDecreasePerSecond * Time.deltaTime); //안전지데가 아닐 떄 감소
        }
        else
        {
            HealSanity(sanityRegenPerSecondInSafeZone * Time.deltaTime);
        }
    }
    void OnEnable()
    {
        if (health == null) health = GetComponent<Health>();
        if (health != null) health.onDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        if (health != null) health.onDamaged -= HandleDamaged;
    }
    public void DecreaseSanity(float amount)
    {
        if (amount <= 0f) return;
        Current = Mathf.Clamp(Current - amount, 0f, max);
        Notify();
    }
    public void HandleDamaged(float amount)
    {
        if (Time.time < lastDamageSanityTime + damageSanityCooldown) return;
        lastDamageSanityTime = Time.time;

        float finalAmount = amount;
        DecreaseSanity(finalAmount);
    }
    private void Notify()
    {
        onSanityChanged?.Invoke(Current, max);
    }
    public void HealSanity(float amount)
    {
        if (amount <= 0f) return;
        if (Current >= max) return;

        float before = Current;
        Current = Mathf.Clamp(Current + amount, 0f, max);

        if (!Mathf.Approximately(before, Current))
            Notify();
    }
    private void Panicked()
    {
        //여기 터널링 효과 + 화면 흔들림 + 환청
    }
}
