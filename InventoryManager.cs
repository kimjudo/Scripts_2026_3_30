
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public InventorySlot[] mainWeaponSlots;
    public InventorySlot[] secondaryWeaponSlots;
    public InventorySlot[] grenadeSlots;
    [HideInInspector] public InventorySlot selectedSlot;

    public HandManager handManager;
    public ItemDropManager itemDropManager;
    [SerializeField] Animator animator;
    [SerializeField] GameObject playerGameObject;
    [SerializeField] private PlayerContext playerContext;

    public GameObject inventoryItemPrefab;

    [SerializeField] private BulletItem bulletItem;

    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ChangeSelectedSlot(0);
    }

    public void InspectSelectedItem()
    {
        Item receivedItem = GetCurrentSelectedItem();
        if (receivedItem != null)
            Debug.Log("Selected Item: " + receivedItem.name);
        else
            Debug.Log("No item in selected slot");
    }
    public void UseSelectedItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (selectedSlot == null) return;

        var itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot == null || itemInSlot.item == null) return;

        animator.SetTrigger(HandAnimeParams.Use);

        bool used = itemInSlot.item.Use(playerContext); // 루트로 가서 헝거를 찾는데 지금 dontdestroyonload가 root로 되어있어서 그래서 못쓴거임 ;그래서 playerContext를 만들어서 그걸 넘겨주는 방식으로 바꿈
        Debug.Log("UseSelectedItem called with context: " + ctx);
        if (used)
        {
            TryRemoveFromSlot(selectedSlot);
            handManager.RefreshHandForSelectedSlot(selectedSlot);
        }
        else
        {
            Debug.Log("Failed to use item: " + itemInSlot.item.name);
        }
    }


    public void OnSelectSlot(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        string path = ctx.control.path;
        char last = path[path.Length - 1];

        int index = last - '1';

        ChangeSelectedSlot(index);
    }

    public void DropOrRemoveItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (selectedSlot == null) return;

        var invItem = selectedSlot.GetComponentInChildren<InventoryItem>();
        if (invItem == null) return;
        var item = invItem.item;

        // 손에 들고 있으면 최신 상태 저장
        handManager.currentWeapon?.SaveToState();
        WeaponState stateCopy = null;
        if (invItem.weaponState != null)
        {
            stateCopy = new WeaponState
            {
                currentAmmo = invItem.weaponState.currentAmmo,
                reserveAmmo = invItem.weaponState.reserveAmmo,
                fireModeIndex = invItem.weaponState.fireModeIndex
            };
        }
        itemDropManager.SpawnWorldItem(item, 1, stateCopy);
        TryRemoveFromSlot(selectedSlot);
        handManager.RefreshHandForSelectedSlot(selectedSlot);
    }
    void ChangeSelectedSlot(int newValue)
    {
        // 1. 이전 슬롯 선택 해제
        if (selectedSlot != null)
            selectedSlot.DeSelect();

        // 2. 이번에 선택할 슬롯 찾기
        InventorySlot target = null;

        if (newValue < 2)
        {
            target = mainWeaponSlots[newValue];
        }
        else if (newValue == 2)
        {
            target = secondaryWeaponSlots[0];
        }
        else if (newValue == 3)
        {
            target = grenadeSlots[0];
        }
        else if (newValue >= 4)
        {
            newValue -= 4;
            target = inventorySlots[newValue];
        }

        // 3. 실제 선택 적용
        selectedSlot = target;

        if (selectedSlot != null)
            selectedSlot.Select();

        InventoryItem selectedItem = selectedSlot?.GetComponentInChildren<InventoryItem>();

        if (selectedSlot != null)
        {
            handManager.Equip(selectedItem);
        }
        else
        {
            handManager.Unequip();
        }
    }

    private bool TryAddWithStack(InventorySlot[] slots, Item item, WeaponState state = null)
    {
        if (slots == null || slots.Length == 0) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null &&
                itemInSlot.item != null &&
                itemInSlot.item == item &&
                itemInSlot.item.isStackable &&
                itemInSlot.count < itemInSlot.item.maxStackCount)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                Debug.Log("Stacked item: " + item.name);
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                SpawnItem(item, slot, state);
                Debug.Log("Added item: " + item.name);
                if (slot == selectedSlot)
                    handManager.RefreshHandForSelectedSlot(slot);
                return true;
            }
        }

        return false;
    }

    public bool TryRemoveFromSlot(InventorySlot slot)
    {
        if (slot == null) return false;
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot == null) return false;

        if (itemInSlot.item.isStackable)
        {
            if (itemInSlot.count > 1)
            {
                itemInSlot.count--;
                itemInSlot.RefreshCount();
            }
            else
            {
                Destroy(itemInSlot.gameObject);
                Debug.Log("Removed item: " + itemInSlot.item.name);
                if(slot == selectedSlot)
                    handManager.RefreshHandForSelectedSlot(selectedSlot);
            }
            return true;
        }
        else if (!itemInSlot.item.isStackable)
        {
            Destroy(itemInSlot.gameObject);

            if (slot == selectedSlot)
                StartCoroutine(RefreshNextFrame(slot));

            return true;
        }
        return false;
    }
    private IEnumerator RefreshNextFrame(InventorySlot slot)
    {
        yield return null;
        handManager.RefreshHandForSelectedSlot(slot);
    }
    public bool AddItemToSlot(Item item, WeaponState state = null)
    {
        if (item == null) return false;

        switch (item.itemType)
        {
            case ItemType.MainWeapon:
                if (TryAddWithStack(mainWeaponSlots, item, state))
                {
                    return true;
                }
                Debug.Log("MainInventory is full" + item.name);
                    return false;
            case ItemType.SecondaryWeapon:
                if (TryAddWithStack(secondaryWeaponSlots, item, state))
                    return true;
                break;

            case ItemType.Throwable:
                if (TryAddWithStack(grenadeSlots, item))
                    return true;
                break;

        }
        if (TryAddWithStack(inventorySlots, item))
            return true;

        return false;
    }


    public void SpawnItem(Item item, InventorySlot slot, WeaponState state = null)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item, 1, state);
    }


    public Item GetCurrentSelectedItem()
    {
        if (selectedSlot == null)
            return null;

        InventoryItem itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
            return itemInSlot.item;

        return null;
    }
    public Item UseCurrentSelectedItem(bool use)
    {
        if (selectedSlot == null)
            return null;
        InventoryItem itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            if (use)
            {
                TryRemoveFromSlot(selectedSlot);
            }
            return itemInSlot.item;
        }
        return null;
    }

    public int GetTotalAmmo()
    {
        int ammoCount = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || itemInSlot.item == null)
                continue;

            if (itemInSlot.item == bulletItem)
            {
                ammoCount += itemInSlot.count;
            }
        }
        return ammoCount;
    }

    public int ConsumeAmmo(int amount)
    {
        if (amount <= 0) return 0;

        int remaining = amount;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (remaining <= 0) break;

            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || itemInSlot.item == null)
                continue;

            if (itemInSlot.item != bulletItem)
                continue;

            int canUse = Mathf.Min(itemInSlot.count, remaining);//작은걸고르는 함수 

            // 이 슬롯에서 canUse만큼 사용
            itemInSlot.count -= canUse;
            remaining -= canUse;

            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
            }
            else
            {
                itemInSlot.RefreshCount();
            }
        }

        int consumed = amount - remaining; // 실제로 사용된 탄 수
        return consumed;
    }
}
