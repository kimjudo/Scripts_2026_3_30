using UnityEngine;

public class HomeReturnInteractable : MonoBehaviour
{
    public void Interact()
    {
        if (MapTravelController.Instance == null)
        {
            Debug.LogWarning("MapTravelController.Instance가 없습니다.");
            return;
        }

        MapTravelController.Instance.BackToHome();
    }
}