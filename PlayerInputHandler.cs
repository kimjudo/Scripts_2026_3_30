using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private ShoveAbility shoveAbility;
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private InventoryUIController mainInventoryButton;
    //Weapon Input Handlers
    public void ChangeFiringMode(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var weapon = handManager.currentWeapon;
        if (weapon == null) return;

        weapon.ChangeFiringModeIndex();
        Debug.Log("Firing Mode Changed to: " + weapon.currentFiringMode);
    }
    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        var weapon = handManager.currentWeapon;
        if (weapon == null) return;

        weapon.TryReload();
    }
    public void OnFire(InputAction.CallbackContext ctx)
    {
        var weapon = handManager.currentWeapon;
        if (weapon == null) return;

        weapon.OnFire(ctx);
    }
    //weapon Input Handlers end

    //Inventory Input Handlers
    public void InspectSelectedItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        InventoryManager.Instance.InspectSelectedItem();
    }
    public void UseSelectedItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        InventoryManager.Instance.UseSelectedItem(ctx);
    }
    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        InventoryManager.Instance.OnSelectSlot(ctx);
    }
    public void OnDropItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        InventoryManager.Instance.DropOrRemoveItem(ctx);
    }
    //Inventory Input Handlers end

    //InteractionManager Input Handlers
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        interactionManager.OnInteract(ctx);
    }
    //InteractionManager Input Handlers end

    //MainInventoryButton Input Handlers
    public void ShowOrHide(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        mainInventoryButton.Toggle();
    }
    //MainInventoryButton Input Handlers end

    //Grenade Input Handlers
    public void OnThrowGrenade(InputAction.CallbackContext ctx)
    {
        handManager.currentGrenade?.OnThrow(ctx);
    }
    public void OnCookGrenade(InputAction.CallbackContext ctx)
    {
        handManager.currentGrenade?.OnCook(ctx);
    }
    //Grenade Input Handlers end

    //HandManager Input Handlers

    public void OnShove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        shoveAbility.TryShove();
    }
}
