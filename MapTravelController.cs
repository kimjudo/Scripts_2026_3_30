using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTravelController : MonoBehaviour
{
    public static MapTravelController Instance { get; private set; }

    public DestinationState State = new DestinationState();

    public event Action<bool, RectTransform, DestinationDatabase> OnSelectionChanged;

    private DestinationDatabase selectedData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectOrDeselect(DestinationDatabase data, RectTransform buttonRect)
    {
        if (State.currentId == data.Id)
        {
            State.currentId = null;
            selectedData = null;
            OnSelectionChanged?.Invoke(false, buttonRect, data);
            return;
        }

        State.currentId = data.Id;
        selectedData = data;

        OnSelectionChanged?.Invoke(true, buttonRect, data);
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

    public void MoveScene()
    {
        if (selectedData == null) return;

        TryMoveScene(selectedData.targetSceneName, selectedData.targetSpawnId);
    }

    public void BackToHome()
    {
        TryMoveScene("Stage0_Motel", "0");
    }
}