using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;

    // 총 아이템일 때만 의미 있음
    public WeaponState weaponState;

    public void InitialiseItem(Item newItem, int newCount = 1, WeaponState existingState = null)
    {
        item = newItem;
        count = Mathf.Max(1, newCount);

        image.sprite = newItem.image;
        image.enabled = (image.sprite != null);

        var gun = newItem as GunItem; //총기 아이템인지 확인
        if (gun != null)
        {
            //기존 state가 있으면 재사용 없으면 새로 생성
            if (existingState != null)
                weaponState = existingState;
            else
            {
                weaponState = new WeaponState();
                weaponState.currentAmmo = Mathf.Clamp(gun.defaultAmmo, 0, gun.maxAmmo);
            }
        }
        else
        {
            weaponState = null; //무기 아니면 상태 없음
        }

        RefreshCount();
    }

    public void RefreshCount()
    {
        if (countText == null || item == null) return;

        bool show = item.isStackable && count > 1;
        countText.gameObject.SetActive(show);
        if (show) countText.text = count.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;
        if (countText) countText.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        if (countText) countText.raycastTarget = true;
    }
}
