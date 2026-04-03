using UnityEngine;

public class HomeReturnInteractable : MonoBehaviour
{
    public void Interact()
    {
        if (SceneTravelService.Instance == null)
        {
            Debug.LogWarning("SceneTravelService.Instance가 없습니다.");
            return;
        }

        SceneTravelService.Instance.BackToHome(); //여기도 원래는 mpaTravelController에서 함수 호출하는거였는데 바꿈
    }
}