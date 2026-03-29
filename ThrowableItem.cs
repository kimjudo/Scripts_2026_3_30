using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ThrowableItem")]
public class ThrowableItem : Item
{
    public ThrowableType throwableType;
    public AudioClip explosionSound;
}
public enum ThrowableType
{
    None,
    Grenade,
    Molotov,
    Smoke,
}