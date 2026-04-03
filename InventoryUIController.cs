using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainInventory;

    public bool IsOpen => mainInventory != null && mainInventory.activeSelf;

    private void Awake()
    {
        if (mainInventory) mainInventory.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (mainInventory == null) return;
        mainInventory.SetActive(true);
    }

    public void Close()
    {
        if (mainInventory == null) return;
        mainInventory.SetActive(false);
    }
}
