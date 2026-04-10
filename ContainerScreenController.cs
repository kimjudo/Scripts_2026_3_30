using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerScreenController : MonoBehaviour
{
    [SerializeField] private ContainerUIController containerUI;
    [SerializeField] private InventoryUIController inventoryUI;
    [SerializeField] private PlayerInput playerInput;
    private ContainerInventory currentContainer;

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

    private void OpenCurrent()//3
    {
        Rebind();

        containerUI?.Open();
        inventoryUI?.Open();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("UI");

        currentContainer?.LoadSelf();
    }

    public void CloseAll()
    {
        Rebind();

        if (currentContainer != null)
        {
            currentContainer.SaveSelf();
            currentContainer = null;
        }

        containerUI?.Close();
        inventoryUI?.Close();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");
    }

    public void ToggleFor(ContainerInventory targetContainer)//2
    {
        if (targetContainer == null)
        {
            Debug.LogWarning("ToggleFor: targetContainer가 null");
            return;
        }

        // 같은 상자를 다시 누르면 닫기
        if (IsOpen && currentContainer == targetContainer)
        {
            CloseAll();
            return;
        }

        // 다른 상자가 열려 있으면 먼저 저장
        if (IsOpen && currentContainer != null)
        {
            currentContainer.SaveSelf();
        }

        currentContainer = targetContainer;
        OpenCurrent();
    }
}