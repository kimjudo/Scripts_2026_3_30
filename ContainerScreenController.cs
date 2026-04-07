using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerScreenController : MonoBehaviour
{
    [SerializeField] private ContainerUIController containerUI;
    [SerializeField] private InventoryUIController inventoryUI;
    [SerializeField] private PlayerInput playerInput;

    public bool IsOpen => containerUI != null && containerUI.IsOpen;

    private void Awake()
    {
        Rebind();
    }

    private void Rebind()
    {
        if (playerInput == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);

            if (player != null)
                playerInput = player.GetComponent<PlayerInput>();
        }
    }

    public void OpenFor()
    {
        Rebind();

        containerUI?.Open();
        inventoryUI?.Open();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("UI");
    }

    public void CloseAll()
    {
        Rebind();

        containerUI?.Close();
        inventoryUI?.Close();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");
    }

    public void ToggleFor()
    {
        if (IsOpen) CloseAll();
        else OpenFor();
    }
}
