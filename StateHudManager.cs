using UnityEngine;
using TMPro;

public class StateHudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI armorText;

    public void SetArmor(int percent)
    {
        if (armorText) armorText.text = $"Armor: {percent}%";
    }
}