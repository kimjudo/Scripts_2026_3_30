using UnityEngine;

public class BossZombieAttack : ZombieAttack
{
        [SerializeField] private StatusEffects.DamageType heavyAttackDamageType = StatusEffects.DamageType.Normal;

        int heavyDamage = 50;
        public void DealHeavyDamageEvent()
    {
        if (attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, targetMask);

        foreach (var hit in hits)
        {
            var dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(heavyDamage, heavyAttackDamageType);
                return; // 한 번만 때리게
            }
        }
    }
}
