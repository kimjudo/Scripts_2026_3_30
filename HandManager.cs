using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class HandManager : MonoBehaviour
{
    public Transform itemSpawnPoint;
    public Transform weaponSpawnPoint;
    public Transform throwPoint;
    private GameObject currentModel;

    [HideInInspector]public Weapon currentWeapon;
    [HideInInspector]public ThrowableInHand currentGrenade;
    [HideInInspector]public InventoryItem currentInventoryItem;

    public Camera playerCamera;

    public HandAnimator handAnimator;
    public InventoryManager inventoryManager;

    public void RefreshHandForSelectedSlot(InventorySlot slot)
    {
        if (slot == inventoryManager.selectedSlot)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                Equip(inventoryItem);
            }
            else
            {
                Unequip();
            }
        }
    }
    public void Equip(InventoryItem invenItem)
    {
        Unequip(); // 기존 장착 정리

        // 빈손
        if (invenItem == null || invenItem.item == null)
        {
            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, 0);
            currentInventoryItem = null;
            return;
        }

        if (invenItem.item.handPrefab == null)
        {
            Debug.LogWarning("No hand prefab for item: " + invenItem.item.itemName);
            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, 0);
            currentInventoryItem = invenItem;
            return;
        }

        currentInventoryItem = invenItem;

        Transform spawnPoint =
            (invenItem.item.itemType == ItemType.MainWeapon || invenItem.item.itemType == ItemType.SecondaryWeapon)
            ? weaponSpawnPoint
            : itemSpawnPoint;

        currentModel = Instantiate(invenItem.item.handPrefab, spawnPoint);

        currentWeapon = currentModel.GetComponent<Weapon>();

        if (currentWeapon != null)
        {
            var gun = invenItem.item as GunItem;
            if (gun == null)
            {
                Debug.LogError("Weapon component exists, but item is not GunItem!");
            }
            else
            {
                if (currentInventoryItem.weaponState == null)
                {
                    currentInventoryItem.weaponState = new WeaponState();
                    currentInventoryItem.weaponState.currentAmmo = gun.defaultAmmo;
                    currentInventoryItem.weaponState.reserveAmmo = 0;
                }

                currentWeapon.Init(gun, currentInventoryItem.weaponState, playerCamera);
                handAnimator.animator.SetInteger(HandAnimeParams.ItemState, gun.animatorID);
            }
        }
        if (invenItem.item is ThrowableItem grenadeItem)
        {
            currentGrenade = currentModel.GetComponent<ThrowableInHand>();

            if (currentGrenade != null)
                currentGrenade.InitGrenade(playerCamera, throwPoint, grenadeItem);

            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, grenadeItem.animatorID);
        }
        if(invenItem.item is PillItem pillItem)
        {
            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, pillItem.animatorID);
        }
        if(invenItem.item is WaterItem waterItem)
        {
            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, waterItem.animatorID);
        }
        if(invenItem.item is BreadItem breadItem)
        {
            handAnimator.animator.SetInteger(HandAnimeParams.ItemState, breadItem.animatorID);
        }
    }
    public void Unequip()
    {
        if (currentWeapon != null && currentInventoryItem != null)
        {
            currentWeapon.SaveToState();
        }

        currentWeapon = null;
        currentGrenade = null;

        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
        }

        handAnimator.animator.SetInteger(HandAnimeParams.ItemState, 0);
        handAnimator.animator.SetTrigger(HandAnimeParams.Unequip);

        currentInventoryItem = null;
    }
}