using UnityEngine;
using UnityEngine.AI;

public class Death : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] MonoBehaviour[] disableOnDeath; // ZombieAI, ZombieAttack 등 드래그
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] HandAnimator handAnimator;

    void Awake()
    {
        health.onDeath += HandleDeath;
    }

    void HandleDeath()
    {
        if (transform.CompareTag("Player"))
        {
            // Player 죽음 처리 (예: 게임 오버 화면 표시)
            foreach (var b in disableOnDeath)
                if (b) b.enabled = false;
            Debug.Log("Player has died. Game Over.");
            // 추가적인 게임 오버 로직을 여기에 작성
        }
        if (transform.CompareTag("Zombie"))
        {
            // AI/공격/기타 스크립트 끄기
            foreach (var b in disableOnDeath)
                if (b) b.enabled = false;

            // 이동 멈추기
            if (agent)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
        }
        animator.SetTrigger("Dead");
    }
}
