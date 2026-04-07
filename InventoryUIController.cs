using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainInventory;
    [SerializeField] private PlayerInput playerInput;

    public bool IsOpen => mainInventory != null && mainInventory.activeSelf;

    private void Awake()
    {
        if (mainInventory) mainInventory.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen) {
            Close();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            Open();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            playerInput.SwitchCurrentActionMap("UI");
        }
    }

    public void Open()
    {
        if (mainInventory == null) return;
        mainInventory.SetActive(true);
    }

    public void Close()
    {
        if (mainInventory == null) return;
        mainInventory.SetActive(false);
    }
}
