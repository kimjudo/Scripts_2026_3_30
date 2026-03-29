using UnityEngine;
using UnityEngine.InputSystem;

public class MainInventoryButton : MonoBehaviour
{
    [SerializeField] private GameObject mainInventory;
    
    public static MainInventoryButton Instance { get; private set; }
    public static bool InventoryOpen { get; private set; }

    [SerializeField] PlayerInput playerInput;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Toggle()
    {
        if (mainInventory == null) return;
        
        bool open = !mainInventory.activeSelf;
        mainInventory.SetActive(open);
        
        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.Confined : CursorLockMode.Locked;
        
        InventoryOpen = open;

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(open ? "UI" : "Player");
        }
    }
    
    private void Start()
    {
        if (mainInventory) mainInventory.SetActive(false);
        InventoryOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
}
