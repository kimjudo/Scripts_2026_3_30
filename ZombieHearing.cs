using UnityEngine;
using static NoiseSystem;

public class ZombieHearing : MonoBehaviour
{
    [SerializeField] private ZombieAI ai;

    private void Awake()
    {
        if (ai == null) ai = GetComponent<ZombieAI>();
    }

    public void HearNoise(Vector3 noisePos, NoiseType type, float loudness)
    {
        if (ai == null) return;

        ai.OnHeardNoise(noisePos, type, loudness);
    }
}
