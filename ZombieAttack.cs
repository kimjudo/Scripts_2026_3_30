using UnityEngine;

/// <summary>
/// 공격 전담 컴포넌트 (구 ZombieAttack + ZombieAttackAnimation 통합)
/// - AI 연동: InRange, IsLocked, TryAttack(), IsTimedOut(), Lock()
/// - 실제 피해: DealDamageEvent() (Animator Event)
/// - 완료 알림: OnAttackComplete 이벤트
/// ZombieAI는 이 컴포넌트 하나만 참조하면 됩니다.
/// </summary>
public class ZombieAttack : MonoBehaviour
{
    // ── AI 연동 ───────────────────────────────────────────
    [Header("Attack")]
    public float attackRange    = 1.8f;
    public float attackCooldown = 1.2f;

    [Header("Safety")]
    [Tooltip("애니 이벤트 누락 시 자동으로 Chase로 복귀하는 최대 대기 시간")]
    public float attackTimeout = 3f;

    // ── 데미지 ────────────────────────────────────────────
    [Header("Damage")]
    public Transform attackPoint;
    public LayerMask targetMask;
    public int       damage     = 10;
    [SerializeField]
    private StatusEffects.DamageType damageType = StatusEffects.DamageType.Normal;

    [Header("Refs")]
    public Animator animator;

    // ── 프로퍼티 (ZombieAI가 읽음) ────────────────────────
    public bool InRange  { get; private set; }
    public bool IsLocked { get; private set; }

    /// <summary>공격 애니 종료 시 ZombieAI에게 알림</summary>
    public event System.Action OnAttackComplete;

    // ── 내부 ──────────────────────────────────────────────
    Transform player;
    float nextAttackTime;
    float attackStartTime;
    float lockEndTime;

    // ─────────────────────────────────────────────────────

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    /// <summary>ZombieAI.Update()에서 매 프레임 호출</summary>
    public void Tick()
    {
        if (player != null)
            InRange = Vector3.Distance(transform.position, player.position) <= attackRange;

        if (IsLocked && Time.time >= lockEndTime)
            IsLocked = false;
    }

    /// <summary>
    /// 공격 시도. 쿨다운·잠금 중이면 false.
    /// ZombieAI는 Attack 상태 진입 첫 프레임에 한 번만 호출.
    /// </summary>
    public bool TryAttack()
    {
        if (IsLocked) return false;
        if (Time.time < nextAttackTime) return false;

        nextAttackTime = Time.time + attackCooldown;
        attackStartTime = Time.time;

        animator.ResetTrigger(ZombieAnimeParams.Attack);
        animator.SetTrigger(ZombieAnimeParams.Attack);
        return true;
    }

    /// <summary>AttackTick()에서 타임아웃 감지용</summary>
    public bool IsTimedOut() => Time.time - attackStartTime > attackTimeout;

    /// <summary>넉백/스턴 발생 시 외부에서 공격 잠금</summary>
    public void Lock(float duration)
    {
        IsLocked    = true;
        lockEndTime = Time.time + duration;
    }

    // ── Animator Events ───────────────────────────────────

    /// <summary>공격 모션 타격 프레임에 Animator Event로 호출 → 실제 피해</summary>
    public void DealDamageEvent()
    {
        Debug.Log("DealDamageEvent called");

        if (attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, targetMask);

        foreach (var hit in hits)
        {
            var dmg = hit.GetComponentInParent<IDamageable>();
            Debug.Log(hit.name + " hit, IDamageable: " + (dmg != null));
            if (dmg != null)
            {
                dmg.TakeDamage(damage, damageType);
                return; // 한 타깃만 피해
            }
        }
    }

    /// <summary>공격 애니 마지막 프레임에 Animator Event로 호출 → 상태 복귀</summary>
    public void OnAttackAnimFinished()
    {
        OnAttackComplete?.Invoke();
    }

    // ── 기즈모 ────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        // 탐지 사거리
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 타격 구체
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}