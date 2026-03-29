using UnityEngine;

public class HealthTestButton : MonoBehaviour
{
    public Health playerHealth;
    public StatusEffects statusEffects;

    private readonly StatusEffects.DamageType normalDamageType = StatusEffects.DamageType.Normal;
    private readonly StatusEffects.DamageType burnDamageType = StatusEffects.DamageType.Burn;

    public void DealDamage(int damage)
    {
        playerHealth.TakeDamage(damage, normalDamageType);
    }
    public void DealBurnDamage(int damage)
    {
        playerHealth.TakeDamage(damage, burnDamageType);
    }
    public void Heal(int amount)
    {
        playerHealth.Heal(amount);
    }

    public void ClearFire()
    {
        statusEffects.ClearFire();
    }
}
