using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Hunger Hunger { get; private set; }
    [field: SerializeField] public Thirst Thirst { get; private set; }
    [field: SerializeField] public Sanity Sanity { get; private set; }
    [field: SerializeField] public Addiction Addiction { get; private set; }
    [field: SerializeField] public Stamina Stamina { get; private set; }

    void Awake()
    {
        Health = GetComponent<Health>();
        Hunger = GetComponent<Hunger>();
        Thirst = GetComponent<Thirst>();
        Sanity = GetComponent<Sanity>();
        Addiction = GetComponent<Addiction>();
        Stamina = GetComponent<Stamina>();

        PlayerRegistry.Register(this);
    }
    public void OnDestroy()
    {
        PlayerRegistry.Unregister(this);
    }
}
