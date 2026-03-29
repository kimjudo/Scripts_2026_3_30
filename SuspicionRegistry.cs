using System.Collections.Generic;
using UnityEngine;

public static class SuspicionRegistry
{
    private static readonly List<ZombieAI> zombies = new List<ZombieAI>();

    public static void Register(ZombieAI z)
    {
        if (z != null && !zombies.Contains(z))
            zombies.Add(z);
    }

    public static void Unregister(ZombieAI z)
    {
        zombies.Remove(z);
    }

    /// <summary>
    /// 등록된 좀비 중 가장 높은 의심 게이지를 반환.
    /// UI의 긴장감 연출 등에 활용.
    /// </summary>
    public static float GetHighestSuspicion()
    {
        float max = 0f;


        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            var z = zombies[i];

            if (z == null)
            {
                zombies.RemoveAt(i);
                continue;
            }

            if (z.sensor.suspicion > max)
                max = z.sensor.suspicion;
        }
        return max;
    }

    /// <summary>
    /// 확정(IsConfirmed) 상태인 좀비가 하나라도 있으면 true.
    /// </summary>
    public static bool AnyConfirmed()
    {
        foreach (var z in zombies)
        {
            if (z != null && z.sensor != null && z.sensor.IsConfirmed)
                return true;
        }
        return false;
    }
}