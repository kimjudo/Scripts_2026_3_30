using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [Header("Ray 설정")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactMask;

    [Header("참조")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private HandAnimator handAnimator;

    private bool isReaching;
    private float blockReachingUntil;

    private WorldItem currentItem;
    private MapBoard currentMap;
    private TeleportDoor currentDoor;
    private ContainerInteractable currentContainer;
    private HomeReturnInteractable homeReturnInteractable;

    private bool reachedSent;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("InteractionManager: Main Camera 못 찾음. 카메라에 MainCamera태그 확인");
            enabled = false;
            return;
        }

        if (handAnimator == null || handAnimator.animator == null)
        {
            Debug.LogError("InteractionManager: handAnimator(또는 animator) 참조가 없음");
            enabled = false;
            return;
        }

        isReaching = handAnimator.animator.GetBool(HandAnimeParams.isReaching);
    }

    private void Update()
    {
        UpdateLookItem();
    }
    private void UpdateLookItem()
    {
        if (Time.time < blockReachingUntil)
        {
            currentItem = null; currentMap = null; currentDoor = null; homeReturnInteractable = null; currentContainer = null;
            SetReaching(false);

            reachedSent = false;
            return;
        }

        currentItem = null;
        currentMap = null;
        currentDoor = null;
        homeReturnInteractable = null;
        currentContainer = null;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        bool hitSomething = Physics.Raycast(
            ray, out RaycastHit hit, maxDistance, interactMask, QueryTriggerInteraction.Ignore
        );

        if (hitSomething)
        {
            currentItem = hit.transform.GetComponentInParent<WorldItem>();
            if (currentItem == null)
            {
                currentMap = hit.transform.GetComponentInParent<MapBoard>();
                if (currentMap == null)
                    currentDoor = hit.transform.GetComponentInParent<TeleportDoor>();
                    
                    if (currentDoor == null)
                        homeReturnInteractable = hit.transform.GetComponentInParent<HomeReturnInteractable>();
                        
                        if (homeReturnInteractable == null)
                            currentContainer = hit.transform.GetComponentInParent<ContainerInteractable>();
            }
        }

        bool isLookingItem = (currentItem != null);

        if (isLookingItem && !reachedSent)
        {
            handAnimator.animator.ResetTrigger(HandAnimeParams.Reached);
            handAnimator.animator.SetTrigger(HandAnimeParams.Reached);
            reachedSent = true;
        }

        if (!isLookingItem)
            reachedSent = false;

        SetReaching(isLookingItem);

    }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (currentMap != null)
        {
            currentMap.InteractWithMap();
            return;
        }
        if (currentDoor != null)
        {
            currentDoor.InteractWithDoor();
            return;
        }

        if (homeReturnInteractable != null)
        {
            homeReturnInteractable.Interact();
            return;
        }
        if(currentContainer != null)
        {
            currentContainer.InteractWithContainer();
            return;
        }

        if (currentItem == null)
        {
            Debug.Log("현재 보고 있는 WorldItem 없음");
            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogError("InteractionManager: inventoryManager 참조가 없음");
            return;
        }

        WeaponState droppedState = null;
        var droppedGun = currentItem.GetComponent<DroppedGunState>();
        if (droppedGun != null)
            droppedState = droppedGun.state;

        bool added = inventoryManager.AddItemToSlot(currentItem.item, droppedState);

        if (added)
        {
            var go = currentItem.gameObject;
            SetReaching(false);

            currentItem = null;

            Destroy(go);
            blockReachingUntil = Time.time + 0.1f;
        }
        else
        {
            Debug.Log("인벤토리 가득, 못 줍는다");
        }
    }

    private void SetReaching(bool value)
    {
        if (isReaching == value) return;
        handAnimator.animator.SetBool(HandAnimeParams.isReaching, value);
        isReaching = value;
    }
}
