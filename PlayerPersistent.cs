using UnityEngine;

public class PlayerPersistent : MonoBehaviour
{
    public static PlayerPersistent Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
