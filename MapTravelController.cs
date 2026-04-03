using UnityEngine;

public class MapTravelController : MonoBehaviour
{
    [SerializeField] private MapSelectionController selectionController;

    public void MoveSelectedScene()
    {
        if (selectionController == null) return;

        var selected = selectionController.SelectedData;
        if (selected == null) return;

        if (SceneTravelService.Instance == null)
        {
            Debug.LogWarning("SceneTravelService.Instance가 없습니다.");
            return;
        }

        SceneTravelService.Instance.TryMoveScene(
            selected.targetSceneName,
            selected.targetSpawnId
        );
        Debug.Log("Moving to scene: " + selected.targetSceneName + " with spawn: " + selected.targetSpawnId);
    }
} 