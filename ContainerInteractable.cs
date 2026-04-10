using UnityEngine;

public class ContainerInteractable : MonoBehaviour
{
    [SerializeField] private ContainerScreenController containerScreen;
    [SerializeField] private ContainerInventory containerInventory;

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

        if (containerInventory == null)
        {
            containerInventory = GetComponent<ContainerInventory>();
        }
    }

    public void InteractWithContainer() //시작
    {
        Rebind();

        if (containerScreen == null)
        {
            Debug.LogWarning("ContainerInteractable: containerScreen이 null");
            return;
        }

        if (containerInventory == null)
        {
            Debug.LogWarning("ContainerInteractable: containerInventory가 null");
            return;
        }

        containerScreen.ToggleFor(containerInventory);
        Debug.Log("ContainerInteractable 인터랙트 성공");
    }
}