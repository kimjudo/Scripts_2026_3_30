using UnityEngine;

public class MapDestinationButton : MonoBehaviour
{
    [SerializeField] private DestinationDatabase data;
    [SerializeField] private MapSelectionController mapSelectionController;

    private RectTransform rect;
    
    void Awake()
    {
        if (mapSelectionController == null)
            mapSelectionController = GetComponentInParent<MapSelectionController>();

        rect = GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        if (mapSelectionController)
        {
            mapSelectionController.SelectOrDeselect(data, rect);
            Debug.Log("MapDestinationButton OnClick / data = " + data);
        } 
        
    }
}
