using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private Item[] items;

    private Dictionary<string, Item> itemLookup;

    private void Awake()
    {
        itemLookup = new Dictionary<string, Item>();
    
        foreach (var item in items)
        {
            if (item == null) continue;

            if (string.IsNullOrWhiteSpace(item.ItemId))
            {
                Debug.LogWarning($"ItemId 비어있음: {item.name}", item);
                continue;
            }

            if (itemLookup.ContainsKey(item.ItemId))
            {
                Debug.LogWarning($"중복 ItemId 발견: {item.ItemId}", item);
                continue;
            }

            itemLookup.Add(item.ItemId, item);
        }
    }
    public Item GetItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId)) return null;

        itemLookup.TryGetValue(itemId, out var item);
        return item;
    }
}
