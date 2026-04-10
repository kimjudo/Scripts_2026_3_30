using UnityEngine;

public class ContainerInventory : MonoBehaviour
{
    [SerializeField] private string containerId;
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private ItemDatabase itemDatabase;

    public string ContainerId => containerId;

    public ContainerSaveData CaptureSaveData()
    {
        ContainerSaveData data = new ContainerSaveData();
        data.containerId = containerId;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null) continue;

            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || itemInSlot.item == null) continue;

            if (string.IsNullOrWhiteSpace(itemInSlot.item.ItemId))
            {
                Debug.LogWarning($"ItemId 없음: {itemInSlot.item.name}", itemInSlot.item);
                continue;
            }

            ItemSlotSaveData slotData = new ItemSlotSaveData
            {
                slotIndex = i,
                itemId = itemInSlot.item.ItemId,
                count = itemInSlot.count
            };

            data.slots.Add(slotData);
        }

        return data;
    }

    public void RestoreSaveData(ContainerSaveData data)
    {
        ClearSlots();

        if (data == null) return;
        if (data.slots == null) return;

        foreach (var slotData in data.slots)
        {
            if (slotData == null) continue;
            if (slotData.slotIndex < 0 || slotData.slotIndex >= slots.Length) continue;

            Item item = itemDatabase.GetItemById(slotData.itemId);
            if (item == null)
            {
                Debug.LogWarning($"ItemDatabase에서 못 찾음: {slotData.itemId}");
                continue;
            }

            SpawnItemInSlot(item, slots[slotData.slotIndex], slotData.count);
        }
    }

    public void SaveSelf()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager 없음");
            return;
        }

        SaveManager.Instance.SaveContainer(CaptureSaveData());
    }

    public void LoadSelf()//4
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager 없음");
            return;
        }

        if (SaveManager.Instance.TryGetContainer(containerId, out var data))
        {
            RestoreSaveData(data);
        }
        else
        {
            ClearSlots();
        }
    }

    private void ClearSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null) continue;

            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                Destroy(itemInSlot.gameObject);
            }
        }
    }

    private void SpawnItemInSlot(Item item, InventorySlot slot, int count)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item, count);
    }
}
