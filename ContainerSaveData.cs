using System.Collections.Generic;

[System.Serializable]
public class ContainerSaveData
{
    public string containerId;
    public List<ItemSlotSaveData> slots = new List<ItemSlotSaveData>();
}
