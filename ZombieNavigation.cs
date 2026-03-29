using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMeshAgent 래핑 전담 컴포넌트.
/// 속도 프로필 전환, 목적지 설정, 순찰/복귀 이동 처리.
/// ZombieAI는 이 클래스를 통해서만 Agent를 건드립니다.
/// </summary>
public class ZombieNavigation : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Transform[] patrolPoints;

    [Header("Speed — Patrol / Return")]
    public float patrolSpeed        = 5f;
    public float patrolAcceleration = 8f;
    public float patrolAngularSpeed = 120f;

    [Header("Speed — Chase")]
    public float chaseSpeed        = 20f;
    public float chaseAcceleration = 20f;
    public float chaseAngularSpeed = 360f;

    [Header("Repath")]
    public float repathInterval = 0.2f;

    // ── 프로퍼티 ──────────────────────────────────────────
    public bool HasPatrolPoints => patrolPoints != null && patrolPoints.Length > 0; //식 본문 프로퍼티 문법 사용 원래는 프로퍼티같은 느낌
    public int PatrolIndex { get; private set; } = 0;
    public Vector3 HomePos { get; private set; }

    /// <summary>목적지에 거의 도착했는지</summary>
    public bool ReachedDestination => //식 본문 프로퍼티 문법 사용 원래는 프로퍼티같은 느낌 
        IsReady() && !agent.pathPending &&
        agent.remainingDistance <= agent.stoppingDistance + 0.3f;
    // ──────────────────────────────────────────────────────

    float nextRepathTime;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        HomePos = transform.position;
    }

    // ── 속도 프로필 ───────────────────────────────────────

    public void ApplyPatrolSpeed()
    {
        if (!IsReady()) return;
        agent.speed        = patrolSpeed;
        agent.acceleration = patrolAcceleration;
        agent.angularSpeed = patrolAngularSpeed;
    }

    public void ApplyChaseSpeed()
    {
        if (!IsReady()) return;
        agent.speed = chaseSpeed;
        agent.acceleration = chaseAcceleration;
        agent.angularSpeed = chaseAngularSpeed;
    }

    // ── 이동 제어 ─────────────────────────────────────────

    public void Stop()
    {
        if (!IsReady()) return;
        agent.isStopped = true;
    }

    public void Resume()
    {
        if (!IsReady()) return;
        agent.isStopped = false;
    }

    /// <summary>repathInterval 쿨다운이 지난 경우에만 목적지 갱신</summary>
    public void SetDestination(Vector3 pos)
    {
        if (!IsReady()) return;
        if (Time.time < nextRepathTime) return;

        nextRepathTime = Time.time + repathInterval;
        agent.SetDestination(pos);
    }

    /// <summary>다음 SetDestination 호출 시 즉시 경로 계산하도록 강제</summary>
    public void ForceRepath() => nextRepathTime = 0f;

    // ── 상태별 Tick ───────────────────────────────────────

    /// <summary>ZombieAI Patrol 상태에서 매 프레임 호출</summary>
    public void PatrolTick()
    {
        if (!HasPatrolPoints) return;

        Transform target = patrolPoints[PatrolIndex];
        if (target == null) return;

        SetDestination(target.position);

        if (ReachedDestination)
            PatrolIndex = (PatrolIndex + 1) % patrolPoints.Length;
    }

    /// <summary>ZombieAI Return 상태에서 매 프레임 호출</summary>
    public void ReturnTick()
    {
        Vector3 dest = HasPatrolPoints
            ? patrolPoints[Mathf.Clamp(PatrolIndex, 0, patrolPoints.Length - 1)].position
            : HomePos;

        SetDestination(dest);
    }

    // ── 유틸 ──────────────────────────────────────────────

    public bool IsReady() => //식 본문 멤버 문법 사용 원래는 메서드같은 느낌
        agent != null && agent.enabled && agent.isOnNavMesh;
}