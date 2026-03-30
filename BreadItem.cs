using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Consumables/Bread")]
public class BreadItem : Item
{
    public float HungerHeal = 15f;

    public override bool Use(PlayerContext player)
    {
        var hunger = player.Hunger;
        
        if (hunger == null) return false;

        hunger.Eat(HungerHeal);
        return true;
    }
}
