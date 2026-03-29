using UnityEngine;
using TMPro;

public class ArmorSystem : MonoBehaviour
{
    public int protectionAmount;

    public void SetArmor(int amount)
    {
        protectionAmount = Mathf.Clamp(amount, 0, 100);
    }

    public float CalculateDamage(int damage)
    {
        float reduction = protectionAmount / 100f;
        float finalDamage = damage * (1f - reduction);
        finalDamage = Mathf.RoundToInt(finalDamage);
        return finalDamage;
    }
}
