using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private StateHudManager stateHud;
    [SerializeField] private GunHudManager gunHud;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public StateHudManager StateHud => stateHud;
    public GunHudManager GunHud => gunHud;
}