using System;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float defaultDuration = 0.35f; // 밀리는 시간
    [SerializeField] private float startSpeed = 6.5f; // 시작 속도 (m/s)
    [SerializeField] private float damping = 0.90f; // 감속(0~1) 낮을수록 빨리 멈춤

    [Header("Collision")]
    [SerializeField] private LayerMask wallMask = ~0; // 벽 레이어
    [SerializeField] private float skin = 0.02f; // 벽에 너무 딱 붙는거 방지
    [SerializeField] private float capsuleRadius = 0.25f; // 좀비 몸통 크기(대충)
    [SerializeField] private float capsuleHeight = 1.6f;

    [Header("Options")]
    [SerializeField] private bool disableAgentWhileActive = true;

    public bool IsActive => _active;
    public bool HitWall => _hitWall;

    public event Action<bool> OnFinished; // bool = hitWall

    NavMeshAgent _agent;
    bool _active;
    bool _hitWall;

    Vector3 _dir;
    float _endTime;
    float _speed;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Begin(Vector3 dir, float distanceAsInput, float? durationOverride = null)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        // 이미 밀리는 중이면 "덮어쓰기" (원하면 누적 방식으로 바꿀 수도 있음)
        _active = true;
        _hitWall = false;

        _dir = dir;

        // distanceAsInput을 너는 shoveForce로 주고 있으니까,
        // 여기선 "거리" -> duration(시간)으로 간단히 환산해도 됨.
        // 더 단순하게: duration 고정으로 두고 speed로 거리 느낌 조절해도 됨.
        float duration = durationOverride ?? defaultDuration;

        // distance 값을 쓰고 싶으면: speed = distance / duration (대충 맞추기)
        // 대신 startSpeed를 기본으로 쓰고 distance로 duration을 약간 늘리는 식도 가능.
        _speed = Mathf.Max(0.01f, distanceAsInput / Mathf.Max(0.01f, duration));
        _speed = Mathf.Max(_speed, startSpeed * 0.5f); // 너무 느려지는 거 방지

        _endTime = Time.time + duration;

        // NavMeshAgent 간섭 끊기
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            if (disableAgentWhileActive)
                _agent.enabled = false;
        }
    }

    void Update()
    {
        if (!_active) return;

        float dt = Time.deltaTime;

        // 시간 종료
        if (Time.time >= _endTime || _speed <= 0.05f)
        {
            Finish();
            return;
        }

        Vector3 move = _dir * (_speed * dt);

        // 벽 체크: 캡슐 캐스트
        if (move.sqrMagnitude > 0.000001f)
        {
            Vector3 p1, p2;
            GetCapsulePoints(out p1, out p2);

            float dist = move.magnitude + skin;

            if (Physics.CapsuleCast(p1, p2, capsuleRadius, _dir, out RaycastHit hit, dist, wallMask, QueryTriggerInteraction.Ignore))
            {
                // 벽 바로 앞까지만 이동
                float allowed = Mathf.Max(0f, hit.distance - skin);
                transform.position += _dir * allowed;

                _hitWall = true;
                Finish();
                return;
            }
        }

        // 이동
        transform.position += move;

        // 감속(지수 감속 느낌)
        _speed *= Mathf.Pow(damping, dt * 60f); // 프레임레이트 영향 줄이기용
    }

    void Finish()
    {
        _active = false;

        // Agent 다시 살리기
        if (_agent != null)
        {
            if (disableAgentWhileActive)
            {
                _agent.enabled = true;
                // 에이전트 위치 싱크(필요할 때가 많음)
                _agent.Warp(transform.position);
            }

            _agent.isStopped = false;
        }

        OnFinished?.Invoke(_hitWall);
    }

    void GetCapsulePoints(out Vector3 p1, out Vector3 p2)
    {
        // 대충 캐릭터 발~머리 사이 캡슐
        Vector3 center = transform.position + Vector3.up * (capsuleHeight * 0.5f);
        float half = Mathf.Max(0.01f, capsuleHeight * 0.5f - capsuleRadius);

        p1 = center + Vector3.up * half;
        p2 = center - Vector3.up * half;
    }
}
