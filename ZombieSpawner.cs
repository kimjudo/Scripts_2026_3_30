using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private ZombieAI zombiePrefab;
    [SerializeField] private PatrolRoute patrolRoute;
    PlayerContext playerContext;

    [SerializeField] private bool SpawnAtStart = true;

    public void Spawn(Vector3 position)
    {
        if (playerContext == null)
            playerContext = PlayerRegistry.Current;

        if (playerContext == null)
        {
            Debug.LogWarning("PlayerContext가 없음");
            return;
        }

        ZombieAI zombie = Instantiate(zombiePrefab, position, Quaternion.identity);
        zombie.Initialize(playerContext);

        if (zombie.nav != null)
        {
            Transform[] points = patrolRoute != null ? patrolRoute.Points : null;
            zombie.nav.SetPatrolPoints(points);
        }
    }

    void Start()
    {
        playerContext = PlayerRegistry.Current;

        if (playerContext == null)
        {
            Debug.LogWarning("플레이어 콘텍스트를 못찾음");
            return;
        }
        if (SpawnAtStart)
        {
            Spawn(transform.position);
        }

    }
}
