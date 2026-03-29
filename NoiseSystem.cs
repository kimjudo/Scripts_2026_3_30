using UnityEngine;

public static class NoiseSystem
{
    public enum NoiseType
    {
        Footstep,
        Jump,
        Land,
        Gunshot,
        Shove,
        Explosion,
    }
    public static void Emit(Vector3 pos, float radius, NoiseType type, float loudness = 1f)
    {
        Collider[] hits = Physics.OverlapSphere(pos, radius, LayerMask.GetMask("Zombie"));

        foreach (var hit in hits)
        {
            var hearing = hit.GetComponentInParent<ZombieHearing>();
            if (hearing == null) continue;

            hearing.HearNoise(pos, type, loudness);
        }
    }
}
