using System;
using UnityEngine;
public class MapSelectionController : MonoBehaviour
{
    public DestinationState State = new DestinationState();

    public event Action<bool, RectTransform, DestinationDatabase> OnSelectionChanged;

    private DestinationDatabase selectedData;
    public DestinationDatabase SelectedData => selectedData;

    public void SelectOrDeselect(DestinationDatabase data, RectTransform buttonRect)
    {
        if (data == null) return;

        if (State.currentId == data.Id)
        {
            State.currentId = null;
            selectedData = null;
            OnSelectionChanged?.Invoke(false, buttonRect, data);
            Debug.Log("Deselected: " + data);
            return;
        }

        State.currentId = data.Id;
        selectedData = data;
        OnSelectionChanged?.Invoke(true, buttonRect, data);
        Debug.Log("Selected: " + data);
    }
}
