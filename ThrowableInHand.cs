using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowableInHand : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GameObject grenadeThrownPrefab;

    private Transform throwPoint;
    private Camera playerCamera;

    [Header("던지는 힘")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 2f;

    [Header("퓨즈 / 쿠킹 설정")]
    [SerializeField] private float baseFuseTime = 3f;
    [SerializeField] private bool canExplodeInHand = true;

    // 이 수류탄의 데이터(SO)
    private ThrowableItem itemData;

    // 상태
    private bool isAiming = false;
    private bool isCooking = false;
    private float cookTime = 0f;

    public void InitGrenade(Camera cam, Transform customThrowPoint, ThrowableItem item)
    {
        playerCamera = cam;
        throwPoint = customThrowPoint;
        itemData = item;
    }

    private void Update()
    {
        if (!isCooking) return;

        cookTime += Time.deltaTime;

        if (canExplodeInHand && cookTime >= baseFuseTime)
        {
            ExplodeInHand();
        }
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            StartAim();
        }
        else if (ctx.canceled)
        {
            Throw();
        }
    }

    public void OnCook(InputAction.CallbackContext ctx)
    {
        // 누르는 순간 쿠킹 시작, 떼면 유지(Throw에서 종료)
        if (ctx.started) StartCooking();
    }

    private void StartAim()
    {
        if (isAiming) return;
        isAiming = true;

        //자세 잡는 애니메이션
    }

    private void Throw()
    {
        if (!isAiming) return;
        isAiming = false;

        float fuse = baseFuseTime;

        if (isCooking)
        {
            fuse = baseFuseTime - cookTime;
            if (fuse <= 0f) fuse = 0.05f;
        }

        bool thrownOk = ThrowGrenade(fuse);

        // 쿠킹 상태 초기화
        isCooking = false;
        cookTime = 0f;

        if (thrownOk)
        {
            var inv = InventoryManager.Instance;
            if (inv != null && inv.selectedSlot != null)
            {
                var slot = inv.selectedSlot;
                inv.TryRemoveFromSlot(slot);
                inv.handManager?.RefreshHandForSelectedSlot(slot);
            }
        }


        //자세 애니 해제
    }

    private bool ThrowGrenade(float fuseTime)
    {
        if (grenadeThrownPrefab == null || throwPoint == null || playerCamera == null)
        {
            Debug.LogWarning("ThrowableInHand: grenadeThrownPrefab/throwPoint/playerCamera 세팅이 안 됨", this);
            return false;
        }

        if (itemData == null)
        {
            Debug.LogError("ThrowableInHand: itemData(ThrowableItem)가 null임. InitGrenade에서 넘겨줘야 함", this);
            return false;
        }

        GameObject obj = Instantiate(grenadeThrownPrefab, throwPoint.position, throwPoint.rotation);

        // 힘 주기
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = playerCamera.transform.forward;
            Vector3 force = dir * throwForce + Vector3.up * upwardForce;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        ThrowableThrown thrown = obj.GetComponent<ThrowableThrown>();
        if (thrown != null)
        {
            thrown.item = itemData;
            thrown.SetFuseTime(fuseTime);
        }
        else
        {
            Debug.LogWarning("ThrowableInHand: grenadeThrownPrefab에 ThrowableThrown 컴포넌트가 없음", obj);
        }

        return true;
    }

    private void StartCooking()
    {
        if (isCooking) return;

        isCooking = true;
        cookTime = 0f;

        //핀 뽑는 애니/사운드
        Debug.Log("수류탄 쿠킹 시작");
    }

    private void ExplodeInHand()
    {
        // 손에서 터지는 걸 “던진 수류탄” 로직 재사용해서 처리
        isCooking = false;
        isAiming = false;

        bool ok = ThrowGrenade(0.05f); // 바로 터지게
        if (ok)
        {
            var inv = InventoryManager.Instance;
            if (inv != null && inv.selectedSlot != null)
            {
                inv.TryRemoveFromSlot(inv.selectedSlot);
                inv.handManager?.RefreshHandForSelectedSlot(inv.selectedSlot);
            }
        }

    }
}
