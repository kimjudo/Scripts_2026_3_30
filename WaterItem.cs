using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Consumables/Water")]
public class WaterItem : Item
{
    public float ThirstHeal = 15f;

    public override bool Use(GameObject user)
    {
        var thirst = user.GetComponent<Thirst>();
        if (thirst == null) return false;

        thirst.Drink(ThirstHeal);
        return true;
    }
}
