using UnityEngine;
using UnityEngine.EventSystems;


public class DarkBackground : MonoBehaviour, IDropHandler
{
    public InventoryManager inventoryManager;
    public ItemDropManager itemDropManager;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();

        inventoryManager.handManager.currentWeapon?.SaveToState();
        WeaponState stateCopy = null;
        if (inventoryItem.weaponState != null)
        {
            stateCopy = new WeaponState
            {
                currentAmmo = inventoryItem.weaponState.currentAmmo,
                reserveAmmo = inventoryItem.weaponState.reserveAmmo,
                fireModeIndex = inventoryItem.weaponState.fireModeIndex
            };
        }
        itemDropManager.SpawnWorldItem(inventoryItem.item, inventoryItem.count, stateCopy);

        Destroy(inventoryItem.gameObject);
        inventoryManager.handManager.Unequip();
    }
    
}
