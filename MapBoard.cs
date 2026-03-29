using UnityEngine;

public class MapBoard : MonoBehaviour
{
    private MapUIController mapUI;

    void Awake()
    {
        Rebind();
    }

    void OnEnable()
    {
        Rebind();
    }

    void Rebind()
    {
        if (mapUI == null)
        {
            mapUI = FindFirstObjectByType<MapUIController>(FindObjectsInactive.Include);

            if (mapUI == null)
            {
                Debug.LogWarning("MapBoard: MapUIController를 찾지 못했습니다.");
            }
        }
    }

    public void InteractWithMap()
    {
        Rebind();

        if (mapUI == null)
        {
            Debug.LogWarning("MapBoard: mapUI가 null이라 인터랙트 불가");
            return;
        }

        Debug.Log("MapBoard 인터랙트 성공");
        mapUI.Toggle();
    }
}