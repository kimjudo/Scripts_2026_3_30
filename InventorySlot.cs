using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventorySlot;

public class InventorySlot : MonoBehaviour,IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    public SlotType slotType = SlotType.Any;

    public enum SlotType
    {
        Any,
        MainWeapon,
        SecondaryWeapon,
        Grenade,
        Consumable
    }

    private void Start()
    {
        DeSelect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }
    public void DeSelect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount != 0) return;

        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();

        if (!CanAccept(inventoryItem))
        {
            Debug.Log("Wrong Type");
            return;
        }
        inventoryItem.parentAfterDrag = transform;
    }

    private bool CanAccept(InventoryItem item)
    {
        Item data = item.item;

        if (slotType == SlotType.Any)
        {
            if (data.itemType == ItemType.MainWeapon)
                return false;

            return true;
        }
        
        
        switch (slotType)
        {
            case SlotType.MainWeapon:
                return data.itemType == ItemType.MainWeapon;

            case SlotType.SecondaryWeapon:
                return data.itemType == ItemType.SecondaryWeapon;

            case SlotType.Grenade:
                return data.itemType == ItemType.Throwable;

            case SlotType.Consumable:
                return data.itemType == ItemType.Consumable;

            default:
                return false;
        }
    }
}

