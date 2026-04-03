using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTravelService : MonoBehaviour
{
    public static SceneTravelService Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TryMoveScene(string sceneName, string spawnId)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;
        if (string.IsNullOrEmpty(spawnId)) return false;

        SceneTravelData.targetSpawnId = spawnId;
        SceneTravelData.hasPendingSpawn = true;

        SceneManager.LoadScene(sceneName);
        return true;
    }

    public void BackToHome()
    {
        TryMoveScene("Stage0_Motel", "0");
    }
}
