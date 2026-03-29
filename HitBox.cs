using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float damageMultiplier = 0.5f;

    [SerializeField] private Health health;

    public void OnHit(int damage, StatusEffects.DamageType damageType)
    {
        damage = Mathf.RoundToInt(damage * damageMultiplier);
        health.TakeDamage(damage, damageType);
    }
}
