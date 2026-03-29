using UnityEngine;

public class MapDestinationButton : MonoBehaviour
{
    [SerializeField] private DestinationDatabase data;
    [SerializeField] private MapTravelController travel;

    private RectTransform rect;
    
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnClick()
    {
        if (travel) travel.SelectOrDeselect(data, rect);
    }
}
