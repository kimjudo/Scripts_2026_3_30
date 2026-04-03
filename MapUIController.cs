using UnityEngine;
using UnityEngine.InputSystem;

public class MapUIController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private PlayerInput playerInput;
    public bool IsOpen => panel != null && panel.activeSelf;
    void Awake()
    {
        Rebind();
    }
    void OnEnable()
    {
        Rebind();
    }
    void Rebind()
    {
        if (panel == null)
        {
            Debug.LogWarning("MapUIController: panel 참조가 없습니다.");
        }

        if (playerInput == null)
        {
            playerInput = GetComponentInParent<PlayerInput>();

            if (playerInput == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerInput = player.GetComponent<PlayerInput>();
            }
        }
    }
    void Start()
    {
        if (panel) panel.SetActive(false);
    }
    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    public void Open()
    {
        Rebind();
        if (panel == null) return;

        panel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (playerInput) playerInput.SwitchCurrentActionMap("UI");
    }

    public void Close()
    {
        Rebind();
        if (panel == null) return;

        panel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (playerInput) playerInput.SwitchCurrentActionMap("Player");
    }
}
