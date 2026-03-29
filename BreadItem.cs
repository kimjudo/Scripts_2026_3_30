using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Consumables/Bread")]
public class BreadItem : Item
{
    public float HungerHeal = 15f;

    public override bool Use(GameObject user)
    {
        var hunger = user.GetComponent<Hunger>();
        if (hunger == null) return false;

        hunger.Eat(HungerHeal);
        return true;
    }
}
