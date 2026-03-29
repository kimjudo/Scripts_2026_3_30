using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneSpawner : MonoBehaviour
{
    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!SceneTravelData.hasPendingSpawn)
            return;

        var points = Object.FindObjectsByType<SpawnPoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var point in points)
        {
            if (point.spawnId == SceneTravelData.targetSpawnId)
            {
                if (cc != null) cc.enabled = false;

                transform.SetPositionAndRotation(
                    point.transform.position,
                    point.transform.rotation
                );

                if (cc != null) cc.enabled = true;

                SceneTravelData.hasPendingSpawn = false;
                return;
            }
        }

        Debug.LogWarning("스폰포인트를 못 찾음: " + SceneTravelData.targetSpawnId);
        SceneTravelData.hasPendingSpawn = false;
    }
}