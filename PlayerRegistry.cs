using Unity.VisualScripting;
using UnityEngine;

public static class PlayerRegistry
{
    public static PlayerContext Current { get; private set; }

    public static void Register(PlayerContext context)
    {
        Current = context;
    }

    public static void Unregister(PlayerContext context)
    {
        if (Current == context)
            Current = null;
    }
}
