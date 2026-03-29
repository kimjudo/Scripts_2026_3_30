using System;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenPerSec = 15f;
    [SerializeField] private float regenDelay = 0.5f;//회복 대기 시간
    [SerializeField] private BarUI staminaUI;

    public float Max => maxStamina;
    public float Current { get; private set; }

    public event Action<float, float> onStaminaChanged;

    private float regenTimer;

    private void Awake()
    {
        Current = maxStamina;
    }

    private void Start()
    {
        // UI가 준비된 다음에 호출
        staminaUI?.SetNormalized(Current / maxStamina);
        onStaminaChanged?.Invoke(Current, maxStamina);
    }

    private void Update()
    {
        // 회복
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            staminaUI.SetNormalized(Current / maxStamina);
            return;
        }

        if (Current < maxStamina)
        {
            Current = Mathf.Min(maxStamina, Current + regenPerSec * Time.deltaTime);
            Notify();
        }
        staminaUI.SetNormalized(Current / maxStamina);
    }

    public bool CanSpend(float amount) => amount > 0f && Current >= amount;

    public bool Spend(float amount)
    {
        if (!CanSpend(amount)) return false;

        Current = Mathf.Max(0f, Current - amount);
        regenTimer = regenDelay;
        Notify();
        return true;
    }

    private void Notify()
    {
        onStaminaChanged?.Invoke(Current, maxStamina);
    }
}
