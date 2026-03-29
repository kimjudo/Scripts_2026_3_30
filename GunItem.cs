using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GunItem")]
public class GunItem : Item
{
    public FiringMode[] availableFiringModes;
    public FiringMode defaultFiringMode = FiringMode.Single;
    public WeaponType weaponType;

    public float fireDelay;
    public int bulletPerBurst = 3;
    public float spreadIntensity;

    public AudioClip shootingSound;
    public AudioClip reloadSound;

    public float damage;

    public float reloadTime;
    public int maxAmmo;
    public int defaultAmmo;
}
public enum WeaponType
{
    Pistol,
    Rifle,
    Shotgun,
    Sniper,
    SMG,
}
public enum FiringMode
{
    Single,
    Burst,
    Auto,
    Safe,
}