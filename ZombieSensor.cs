using UnityEngine;
using static NoiseSystem;

/// <summary>
/// 감지 전담 컴포넌트.
/// 시야각, 후방 감지, 의심 게이지, 소음 반응을 처리하고
/// ZombieAI가 읽어갈 프로퍼티만 노출합니다.
/// </summary>
public class ZombieSensor : MonoBehaviour
{
    [Header("Vision")]
    public float frontViewDistance = 18f;
    [Range(0, 180)] public float frontFov = 140f;
    public float rearDetectRadius = 4f;
    public LayerMask obstacleMask;

    [Header("Suspicion")]
    [Range(0f, 1f)] public float suspicion = 0f;
    public float suspicionGainPerSec = 0.7f;
    public float suspicionLossPerSec = 0.35f;
    public float confirmThreshold = 1.0f;

    [Header("Lost Sight")]
    public float lostSightGiveUpTime = 1.0f;
    public Transform playerEyeTarget;

    // ── ZombieAI가 읽을 프로퍼티 ──────────────────────────
    public bool IsConfirmed      => suspicion >= confirmThreshold;
    public bool LostForTooLong   => lostSightTimer >= lostSightGiveUpTime;
    public bool CanSee           { get; private set; }
    public Vector3 LastKnownPos  { get; private set; }
    // ──────────────────────────────────────────────────────

    Transform player;
    float lostSightTimer;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    /// <summary>ZombieAI.Update()에서 매 프레임 호출</summary>
    public void Tick()
    {
        if (player == null) return;

        CanSee = CheckCanSee();

        if (CanSee)
        {
            LastKnownPos  = player.position;
            suspicion     = Mathf.Clamp01(suspicion + suspicionGainPerSec * Time.deltaTime);
            lostSightTimer = 0f;
        }
        else
        {
            suspicion      = Mathf.Clamp01(suspicion - suspicionLossPerSec * Time.deltaTime);
            lostSightTimer += Time.deltaTime;
        }
    }

    /// <summary>Return 상태에서 의심 게이지를 빠르게 내릴 때</summary>
    public void DrainSuspicion(float multiplier = 1.5f)
    {
        suspicion = Mathf.Clamp01(suspicion - suspicionLossPerSec * multiplier * Time.deltaTime);
    }

    /// <summary>GoReturn 시 게이지 즉시 초기화</summary>
    public void ResetSuspicion()
    {
        suspicion      = 0f;
        lostSightTimer = 0f;
    }

    /// <summary>NoiseSystem에서 소음을 받을 때</summary>
    public void OnHeardNoise(Vector3 noisePos, NoiseType type, float loudness)
    {
        suspicion    = Mathf.Clamp01(suspicion + loudness);
        LastKnownPos = noisePos;
    }

    // ── 내부 감지 로직 ────────────────────────────────────

    bool CheckCanSee()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float flatDist = flatToPlayer.magnitude;

        // 1) 후방 근거리
        if (flatDist <= rearDetectRadius) return true;

        // 2) 전방 거리 제한
        if (flatDist > frontViewDistance) return false;

        // 3) 시야각
        float angle = Vector3.Angle(transform.forward, flatToPlayer.normalized);
        if (angle > frontFov * 0.5f) return false;

        // 4) 장애물 차단
        return HasLineOfSight();
    }

bool HasLineOfSight()
{
    if (playerEyeTarget == null) return false;

    Vector3 origin = transform.position + Vector3.up * 1.6f;
    Vector3 targetPos = playerEyeTarget.position;
    Vector3 dir = targetPos - origin;
    float dist = dir.magnitude;

    if (dist <= 0.01f) return true;

    return !Physics.Raycast(origin, dir.normalized, dist, obstacleMask);
}

    // ── 기즈모 ────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rearDetectRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, frontViewDistance);
    }
}