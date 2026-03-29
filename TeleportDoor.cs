using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    [SerializeField] private MapTravelController mapTravelCon;

    public void InteractWithDoor()
    {
        if(mapTravelCon == null)return;
        mapTravelCon.MoveScene();
    }
}
