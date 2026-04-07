using UnityEngine;

public class ContainerInteractable : MonoBehaviour
{
    [SerializeField] private ContainerScreenController containerScreen;

    private void Awake()
    {
        Rebind();
    }

    private void OnEnable()
    {
        Rebind();
    }

    private void Rebind()
    {
        if (containerScreen == null)
        {
            containerScreen = FindFirstObjectByType<ContainerScreenController>(FindObjectsInactive.Include);

            if (containerScreen == null)
            {
                Debug.LogWarning("ContainerInteractable: ContainerScreenController를 찾지 못했습니다.");
            }
        }
    }

    public void InteractWithContainer()
    {
        Rebind();

        if (containerScreen == null)
        {
            Debug.LogWarning("ContainerInteractable: containerScreen이 null이라 인터랙트 불가");
            return;
        }
        containerScreen.ToggleFor();
        Debug.Log("ContainerInteractable 인터랙트 성공");

    }
}