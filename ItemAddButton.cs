using UnityEngine;

public class ItemAddButton : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;
    
    public void PickupItems(int id)
    {
        bool result = inventoryManager.AddItemToSlot(itemsToPickup[id]);
        if(result)
        {
            Debug.Log("Item added to inventory.");
        }
        else
        {
            Debug.Log("Inventory full. Could not add item.");
        }
    }
}
