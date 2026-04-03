using UnityEngine;
using static NoiseSystem;

/// <summary>
/// 좀비 상태 머신.
/// "어떤 상태로 갈지" 판단만 담당합니다.
/// 실제 이동·감지·공격·넉백은 각 전담 컴포넌트가 처리합니다.
///
/// 컴포넌트 역할 분리:
///   ZombieSensor     — 시야/소음/의심 게이지
///   ZombieNavigation — NavMeshAgent 래핑, 속도·경로
///   ZombieAttack     — 공격 판정·쿨다운·애니 트리거
///   Knockback        — 물리 밀림 (기존 유지)
/// </summary>
public class ZombieAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Return, Standing, Attack, Die, Knockback, Stunned }

    [Header("Components")]
    public ZombieSensor sensor;
    public ZombieNavigation nav;
    public ZombieAttack attack;
    public Knockback knockback;
    public Animator animator;
    PlayerContext playerContext;

    [Header("Chase — Give Up")]
    public float giveUpDistance = 60f;
    public float safeZoneGiveUpSeconds = 0.2f;

    [Header("Stunned")]
    public float stunnedDuration = 1f;

    [Header("Debug")]
    public bool debugLogs = false;

    // ── 내부 상태
    State state = State.Patrol;
    Transform player;
    Sanity sanity;
    bool  playerInSafeZone;
    float safeZoneTimer;
    float stunEndTime;
    bool  attackTriggeredThisTurn; // Attack 상태에서 한 번만 TryAttack 호출

    void Awake()
    {
        // 자동 참조 수집
        if (sensor == null) sensor = GetComponent<ZombieSensor>();
        if (nav == null) nav = GetComponent<ZombieNavigation>();
        if (attack == null) attack = GetComponent<ZombieAttack>();
        if (knockback == null) knockback = GetComponent<Knockback>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // 이벤트 구독
        attack.OnAttackComplete += OnAttackFinished;
        knockback.OnFinished += OnKnockbackFinished;

        SetState(State.Patrol);
    }

    public void Initialize(PlayerContext context)
    {
        playerContext = context;

        if (playerContext == null)
        {
            player = null;
            sanity = null;
            return;
        }

        player = playerContext.transform;
        sanity = playerContext.Sanity;
    }

    void Update()
    {
        if (player == null) return;

        // 서브 컴포넌트 틱
        sensor.Tick();
        attack.Tick();

        bool vulnerable =
            state != State.Chase&&
            state != State.Attack&&
            state != State.Knockback&&
            state != State.Stunned;

        if (vulnerable && sensor.IsConfirmed)
        {
            sanity?.DecreaseSanity(sanity.DecreaseWhenCaught);
            safeZoneTimer = 0f;
            SetState(State.Chase);
        }

        float speed01 = state switch
        {
            State.Patrol or State.Return => 0.5f,
            State.Chase                  => 1f,
            _                            => 0f
        };
        animator.SetFloat(ZombieAnimeParams.Velocity, speed01);

        switch (state)
        {
            case State.Patrol: PatrolTick(); break;
            case State.Chase: ChaseTick(); break;
            case State.Return: ReturnTick(); break;
            case State.Standing: StandingTick(); break;
            case State.Attack: AttackTick(); break;
            case State.Stunned:
                if (Time.time >= stunEndTime) SetState(State.Chase);
                break;
        }
    }

    // ── 상태별 Tick ───────────────────────────────────────

    void PatrolTick()
    {
        if (!nav.HasPatrolPoints) { SetState(State.Standing); return; }
        nav.PatrolTick();
    }

    void StandingTick()
    {
        if (nav.HasPatrolPoints) SetState(State.Patrol);
    }

    void ChaseTick()
    {
        // 시야 끊긴 지 너무 오래됨
        if (sensor.LostForTooLong) { GoReturn(); return; }

        // 거리 포기
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist >= giveUpDistance) { GoReturn(); return; }

        // 안전지대 포기
        if (playerInSafeZone)
        {
            safeZoneTimer += Time.deltaTime;
            if (safeZoneTimer >= safeZoneGiveUpSeconds) { GoReturn(); return; }
        }
        else
        {
            safeZoneTimer = 0f;
        }

        // 공격 범위 안
        if (attack.InRange && !attack.IsLocked)
        {
            SetState(State.Attack);
            return;
        }

        nav.SetDestination(player.position);
    }

    void ReturnTick()
    {
        sensor.DrainSuspicion();
        nav.ReturnTick();
        if (nav.ReachedDestination) SetState(State.Patrol);
    }

    void AttackTick()
    {
        // 상태 진입 첫 프레임에 한 번만 공격 시도
        if (!attackTriggeredThisTurn)
        {
            attack.TryAttack();
            attackTriggeredThisTurn = true;
        }

        // 안전망: 애니 이벤트 누락 시 자동 복귀
        if (attack.IsTimedOut()) SetState(State.Chase);
    }

    // ── 상태 전환 ─────────────────────────────────────────

    void SetState(State next)
    {
        if (state == next) return;
        state = next;

        attackTriggeredThisTurn = false;

        bool navReady = nav.IsReady();

        if (navReady)
        {
            switch (state)
            {
                case State.Patrol:
                    nav.ApplyPatrolSpeed();
                    nav.Resume();
                    break;

                case State.Chase:
                    nav.ApplyChaseSpeed();
                    nav.ForceRepath();
                    nav.Resume();
                    break;

                case State.Return:
                    nav.ApplyPatrolSpeed();
                    nav.ForceRepath();
                    nav.Resume();
                    break;

                case State.Standing:
                case State.Attack:
                case State.Knockback:
                case State.Stunned:
                    nav.Stop();
                    break;
            }
        }

        // Knockback 애니는 SetState 안에서만 트리거 (이중 호출 방지)
        if (state == State.Knockback)
        {
            animator.ResetTrigger(ZombieAnimeParams.Knockback);
            animator.SetTrigger(ZombieAnimeParams.Knockback);
        }

        if (debugLogs)
            Debug.Log($"[ZombieAI] → {state}  (NavReady={navReady})", this);
    }

    void GoReturn()
    {
        sensor.ResetSuspicion();
        safeZoneTimer = 0f;
        nav.ForceRepath();
        SetState(State.Return);
    }

    // ── 외부 인터페이스 ───────────────────────────────────

    /// <summary>플레이어가 박을 때 외부에서 호출</summary>
    public void ApplyShove(Vector3 dir, float distance)
    {
        if (knockback == null) return;
        SetState(State.Knockback);
        attack.Lock(0.6f);

        knockback.OnFinished -= OnKnockbackFinished;
        knockback.OnFinished += OnKnockbackFinished;
        knockback.Begin(dir, distance, 0.5f);
    }

    /// <summary>안전지대 트리거에서 호출</summary>
    public void SetPlayerInSafeZone(bool inZone) => playerInSafeZone = inZone;

    /// <summary>NoiseSystem에서 소음 전달</summary>
    public void OnHeardNoise(Vector3 noisePos, NoiseType type, float loudness)
    {
        sensor.OnHeardNoise(noisePos, type, loudness);
        if (sensor.IsConfirmed) SetState(State.Chase);
    }

    // ── 이벤트 콜백 ───────────────────────────────────────

    void OnAttackFinished()
    {
        if (state == State.Attack) SetState(State.Chase);
    }

    void OnKnockbackFinished(bool hitWall)
    {
        stunEndTime = Time.time + stunnedDuration;
        attack.Lock(stunnedDuration);
        SetState(State.Stunned);
    }

    // ── SuspicionRegistry ─────────────────────────────────

    void OnEnable()  => SuspicionRegistry.Register(this);
    void OnDisable() => SuspicionRegistry.Unregister(this);
}