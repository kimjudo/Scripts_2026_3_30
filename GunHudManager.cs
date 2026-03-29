using UnityEngine;
using TMPro;

public class GunHudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    public void SetAmmo(int current, int reserve)
    {
        if (!ammoText) return;
        ammoText.text = $"{current} / {reserve}";
    }
}