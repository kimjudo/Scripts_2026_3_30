using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Consumables/Pill")]
public class PillItem : Item
{
    public float sanityHeal = 15f;
    public float addictionGain = 10f;

    public override bool Use(PlayerContext player)
    {
        var sanity = player.Sanity;
        var addiction = player.Addiction;
        
        if (sanity == null || addiction == null) return false;

        sanity.HealSanity(sanityHeal);
        addiction.increaseAddiction(addictionGain);
        return true;
    }
}
