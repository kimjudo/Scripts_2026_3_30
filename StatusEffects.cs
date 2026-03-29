using System.Collections;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private Health playerHealth;

    private bool isBurning = false;
    private Coroutine burnRoutine;

    [SerializeField] private GameObject burningEffect;
    private GameObject burningFxInstance;

    public enum DamageType { Normal, Burn, Bile, Stun }

    private void Start()
    {
        playerHealth = GetComponent<Health>();
    }

    public void HandleStatusEffect(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Burn:
                if (!isBurning)
                    burnRoutine = StartCoroutine(BurnDamage());
                break;
        }
    }

    private IEnumerator BurnDamage()
    {
        int burnDamage = 2;
        int ticks = 10;
        float interval = 1f;

        isBurning = true;

        if (burningEffect != null)
            burningFxInstance = Instantiate(burningEffect, transform.position, Quaternion.identity, transform);

        for (int i = 0; i < ticks; i++)
        {
            playerHealth.ApplyRawDamage(burnDamage);

            yield return new WaitForSeconds(interval);

            if (playerHealth.IsDead) break;
        }

        isBurning = false;
        burnRoutine = null;

        if (burningFxInstance != null) Destroy(burningFxInstance);
    }

    public void ClearFire()
    {
        if (!isBurning) return;

        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        burnRoutine = null;
        isBurning = false;

        if (burningFxInstance != null) Destroy(burningFxInstance);
    }
}

