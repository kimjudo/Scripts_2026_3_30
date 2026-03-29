using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private StatusEffects statusEffects;
    [SerializeField] private ArmorSystem armorSystem;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action onDeath;
    public event Action<float, float> onHealthChanged;
    public event Action<float> onDamaged;

    private void Awake()
    {
        if (statusEffects == null) statusEffects = GetComponent<StatusEffects>();
        if (armorSystem == null) armorSystem = GetComponent<ArmorSystem>();

        CurrentHealth = maxHealth;
    }
    private void Start()
    {
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
    public void TakeDamage(int damage, StatusEffects.DamageType damageType)
    {
        if (IsDead || damage <= 0) return;

        float finalDamage = armorSystem != null
            ? armorSystem.CalculateDamage(damage)
            : damage;

        ApplyRawDamage(finalDamage);

        if (statusEffects != null)
            statusEffects.HandleStatusEffect(damageType);
        onDamaged?.Invoke(finalDamage);
    }

    public void ApplyRawDamage(float damage)
    {
        if (IsDead || damage <= 0f) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0f, maxHealth);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        onHealthChanged?.Invoke(CurrentHealth, maxHealth);

    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"{gameObject.name} is Dead");
        onDeath?.Invoke();
    }
}

