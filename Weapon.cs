using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    // === 참조들 ===
    private GunItem itemData;
    private WeaponState weaponState;


    private InventoryManager invenManager;
    private HandAnimator handAnimator;

    // --- Fire State ---
    public bool isFiring, ReadyToFire;
    bool allowReset = true;

    float fireDelay;
    int bulletPerBurst;
    float spreadIntensity;
    int burstBulletLeft;

    [SerializeField] Transform bulletSpawn;

    public FiringMode currentFiringMode;
    int fireModeIndex = 0;

    GameObject handWeapon;

    bool firePressed = false;
    Animator animator;
    Camera cam;
    [SerializeField] GameObject muzzleEffects;
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private float tracerLife = 0.2f;

    // --- Reload ---
    float reloadTime;
    int maxAmmo;
    int reserveAmmo;

    int _currentAmmo;
    int currentAmmo
    {
        get => _currentAmmo;
        set
        {
            _currentAmmo = value;
            SetEmptyState();
        }
    }
    public bool IsReloading { get; private set; }
    Coroutine reloadCo;

    public int Debug_CurrentAmmo => currentAmmo;
    public int Debug_StateAmmo => weaponState != null ? weaponState.currentAmmo : -999;
    public int Debug_StateHash => weaponState != null ? weaponState.GetHashCode() : 0;

    // --- Ray ---
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float range = 100f;
    [SerializeField] private StatusEffects.DamageType damageType = StatusEffects.DamageType.Normal;
    public bool CanReload
    {
        get
        {
            if (IsReloading) return false;
            if (currentAmmo >= maxAmmo) return false;
            if (invenManager == null) return false;
            return invenManager.GetTotalAmmo() > 0;
        }
    }
    void Awake()
    {
        ReadyToFire = true;
        animator = GetComponent<Animator>();
        invenManager = InventoryManager.Instance;
        handAnimator = GetComponentInParent<HandAnimator>();
        if (handAnimator == null)
            Debug.LogError("HandAnimator 못 찾음! 부모 체인에 HandAnimator가 있어야 함.", this);
    }

    void Update()
    {
        if (ReadyToFire && firePressed)
        {
            HandleFireInput();
        }
    }
    void UpdateAmmoUI()
    {
        var ui = UIManager.Instance;
        if (ui == null)
            {
                Debug.LogWarning("UIManager.Instance is null");
                return;
            }

            if (ui.GunHud == null)
            {
                Debug.LogWarning("UIManager.GunHud is null (인스펙터 할당/초기화 순서/씬에 존재 여부 확인)");
                return;
            }

            ui.GunHud.SetAmmo(currentAmmo, reserveAmmo);
            UIManager.Instance?.GunHud?.SetAmmo(currentAmmo, reserveAmmo);
    }
    void HandleFireInput()
    {
        if (!ReadyToFire || IsReloading) return;

        if (currentAmmo <= 0)
        {
            PlayEmptySoundAndCooldown();
            firePressed = false;
            return;
        }

        if (currentFiringMode == FiringMode.Burst)
            burstBulletLeft = bulletPerBurst;

        FireWeapon();

        if (currentFiringMode == FiringMode.Single)
            firePressed = false;
    }
    void PlayEmptySoundAndCooldown()
    {
        if (SoundManager.Instance?.emptyMagazineChannel != null && ReadyToFire)
        {
            SoundManager.Instance.emptyMagazineChannel.Play();
            ReadyToFire = false;
            if (allowReset)
            {
                Invoke(nameof(ResetShot), fireDelay);
                allowReset = false;
            }
        }
    }
    public void FireWeapon()
    {
        if (IsReloading) return;
        if (!ReadyToFire) return;
        if (currentAmmo <= 0) return;

        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleEffects != null)
            muzzleEffects.GetComponent<ParticleSystem>()?.Play();

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            handAnimator?.animator?.SetTrigger("Attack");
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayShootingSound(itemData);

        ReadyToFire = false;

        Vector3 dir = CalculateDirectionAndSpread().normalized;
        FireHitscan(dir);

        if (allowReset)
        {
            Invoke(nameof(ResetShot), fireDelay);
            allowReset = false;
        }

        if (currentFiringMode == FiringMode.Burst && burstBulletLeft > 0)
        {
            burstBulletLeft--;
            if (burstBulletLeft > 0 && currentAmmo > 0)
            {
                Invoke(nameof(FireWeapon), fireDelay);
                firePressed = false;
            }
        }
        if (currentAmmo <= 0)
        {
            animator.SetTrigger("Empty");
        }
        NoiseSystem.Emit(transform.position, itemData.soundRange, NoiseSystem.NoiseType.Gunshot, itemData.soundLoudness);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (IsReloading) return; // 리로드 중 발사 차단
        if (MainInventoryButton.InventoryOpen) return;// 인벤토리 열려있을땐 발사 차단

        if (ctx.canceled)
        {
            firePressed = false;
            return;
        }

        if (currentFiringMode == FiringMode.Auto)
        {
            if (ctx.started) firePressed = true;
        }
        else
        {
            if (ctx.performed) firePressed = true;
        }
    }


    IEnumerator ReloadRoutine()
    {
        SoundManager.Instance?.PlayReloadSound(itemData);

        if (currentAmmo == 0)
        {
            animator?.SetTrigger("Reload_Empty");
            handAnimator?.animator?.SetTrigger("Reload_Empty");
        }
        else
        {
            animator?.SetTrigger("Reload_Left");
            handAnimator?.animator?.SetTrigger("Reload_Left");
        }

        IsReloading = true;
        yield return new WaitForSeconds(reloadTime);

        int need = maxAmmo - currentAmmo;
        if (need > 0 && invenManager != null)
        {
            int taken = invenManager.ConsumeAmmo(need);
            currentAmmo += taken;
        }

        reserveAmmo = invenManager != null ? invenManager.GetTotalAmmo() : 0;
        if (weaponState != null)
            weaponState.reserveAmmo = reserveAmmo;

        IsReloading = false;
        reloadCo = null;

        UpdateAmmoUI();
    }

    public bool TryReload()
    {
        if (!CanReload) return false;
        if (reloadCo != null) return false;// 중복 방지
        reloadCo = StartCoroutine(ReloadRoutine());// 시작
        return true;
    }

    void ResetShot()
    {
        ReadyToFire = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 origin = cam != null ? cam.transform.position : bulletSpawn.position;

        var ray = cam != null
            ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0))
            : Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(range);

        Vector3 direction = (targetPoint - origin).normalized;

        Transform spreadBasis = cam != null ? cam.transform : bulletSpawn;
        Vector3 spread =
            spreadBasis.right * Random.Range(-spreadIntensity, spreadIntensity) +
            spreadBasis.up * Random.Range(-spreadIntensity, spreadIntensity);

        return (direction + spread).normalized;
    }
    public void Init(GunItem item, WeaponState state, Camera cam)
    {
        itemData = item;
        state ??= new WeaponState();
        weaponState = state;
        this.cam = cam;

        if (itemData != null)
        {
            handWeapon = itemData.handPrefab;
            fireDelay = itemData.fireDelay;
            bulletPerBurst = itemData.bulletPerBurst;
            spreadIntensity = itemData.spreadIntensity;

            reloadTime = itemData.reloadTime;
            maxAmmo = itemData.maxAmmo;

            if (itemData.availableFiringModes != null && itemData.availableFiringModes.Length > 0)
            {
                int idx = weaponState.fireModeIndex;
                if (idx < 0 || idx >= itemData.availableFiringModes.Length)
                {
                    int defIdx = System.Array.IndexOf(itemData.availableFiringModes, itemData.defaultFiringMode);
                    idx = defIdx >= 0 ? defIdx : 0;
                }
                fireModeIndex = idx;
                currentFiringMode = itemData.availableFiringModes[fireModeIndex];
            }
            else
            {
                fireModeIndex = 0;
                currentFiringMode = FiringMode.Single;
            }
        }

        currentAmmo = weaponState.currentAmmo;

        reserveAmmo = invenManager != null ? invenManager.GetTotalAmmo() : weaponState.reserveAmmo;

        burstBulletLeft = bulletPerBurst;

        UpdateAmmoUI();
    }

    public void SaveToState()
    {
        weaponState ??= new WeaponState();

        weaponState.currentAmmo = currentAmmo;
        weaponState.reserveAmmo = reserveAmmo;
        weaponState.fireModeIndex = fireModeIndex;
    }
    public void ChangeFiringModeIndex()
    {
        if (itemData == null || itemData.availableFiringModes == null || itemData.availableFiringModes.Length == 0)
            return;

        fireModeIndex++;
        if (fireModeIndex >= itemData.availableFiringModes.Length)
            fireModeIndex = 0;

        currentFiringMode = itemData.availableFiringModes[fireModeIndex];

        // state에도 반영
        if (weaponState != null)
            weaponState.fireModeIndex = fireModeIndex;
        handAnimator?.animator?.SetTrigger("Toggle");
    }

    private void FireHitscan(Vector3 dir)
    {
        dir = dir.normalized;
        Vector3 origin = cam != null ? cam.transform.position : bulletSpawn.position;
        Ray ray = new Ray(origin, dir);

        Vector3 endPoint = origin + dir * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            int calculatedDamage = CalculateBulletDamage();
            
            var hitBox = hit.collider.GetComponentInParent<HitBox>();
            if (hitBox != null)
            {
                hitBox.OnHit(calculatedDamage, damageType);
            }
            else
            {
                var d = hit.collider.GetComponentInParent<IDamageable>();
                d?.TakeDamage(calculatedDamage, damageType);
            }
        }
        Vector3 startPoint = bulletSpawn.position;
        SpawnTracer(startPoint, endPoint);
        Debug.DrawRay(origin, dir * range, Color.red, 1f);
    }

    private int CalculateBulletDamage()
    {
        int finalDamage = (int)itemData.damage;

        return finalDamage;
    }

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);

        var tr = tracer.GetComponent<TrailRenderer>();
        if (tr != null)
        {
            tr.Clear();
            tracer.transform.position = start; // 시작
            tracer.transform.position = end;   // 끝(순간이동) -> Trail이 선처럼 남음
        }

        Destroy(tracer, tracerLife);
    }

    void SetEmptyState()
    {
        bool empty = _currentAmmo <= 0;
        animator?.SetBool("isEmpty", empty);
    }
}

