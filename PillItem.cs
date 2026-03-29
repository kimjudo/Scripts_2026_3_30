using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Consumables/Pill")]
public class PillItem : Item
{
    public float sanityHeal = 15f;
    public float addictionGain = 10f;

    public override bool Use(GameObject user)
    {
        var sanity = user.GetComponent<Sanity>();
        var addiction = user.GetComponent<Addiction>();
        if (sanity == null || addiction == null) return false;

        sanity.HealSanity(sanityHeal);
        addiction.increaseAddiction(addictionGain);
        return true;
    }
}
