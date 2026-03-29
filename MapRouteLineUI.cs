using UnityEngine.UI;
using UnityEngine;

public class MapRouteLineUI : MonoBehaviour
{
    [SerializeField] private MapTravelController travel;
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform lineRect;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private Image lineImage;

    [SerializeField] private float thickness = 6f;

    RectTransform target;
    
    void Start()
    {
        lineImage.raycastTarget = false;
    }
    void OnEnable()
    {
        Debug.Log($"MapRouteLineUI OnEnable / travel = {travel}");
        if (travel) travel.OnSelectionChanged += HandleSelectionChanged;
    }

    void OnDisable()
    {
        if (travel) travel.OnSelectionChanged -= HandleSelectionChanged;
    }
    void HandleSelectionChanged(bool selected, RectTransform buttonRect, DestinationDatabase data)
    {
        if (!selected)
        {
            target = null;
            var color = lineImage.color;
            color.a = 0f;
            lineImage.color = color;
            return;
        }

        target = buttonRect;
        var c = lineImage.color;
        c.a = 1f;
        lineImage.color = c;
        UpdateLine();
    }
    void LateUpdate()
    {
        if (target) UpdateLine();
    }
    void UpdateLine()
    {
        Vector2 a = (Vector2)root.InverseTransformPoint(startPoint.position);
        Vector2 b = (Vector2)root.InverseTransformPoint(target.position);

        Vector2 dir = b - a;
        float len = dir.magnitude;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        lineRect.anchoredPosition = a;
        lineRect.sizeDelta = new Vector2(len, thickness);
        lineRect.localRotation = Quaternion.Euler(0, 0, ang);
    }
}
