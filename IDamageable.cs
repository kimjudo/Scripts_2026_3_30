using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, StatusEffects.DamageType damageType);
}
