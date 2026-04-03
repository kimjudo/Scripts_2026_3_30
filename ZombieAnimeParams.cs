using UnityEngine;

public static class ZombieAnimeParams
{
    public static readonly int isWalking = Animator.StringToHash("isWalking");
    public static readonly int isRunning = Animator.StringToHash("isRunning");
    public static readonly int isStanding = Animator.StringToHash("isStanding");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Dying = Animator.StringToHash("Dying");
    public static readonly int Dead = Animator.StringToHash("Dead");
    public static readonly int Velocity = Animator.StringToHash("Velocity");
    public static readonly int Knockback = Animator.StringToHash("Knockback");
}
