using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private Dictionary<string, ContainerSaveData> containerStates = new();

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

    public void SaveContainer(ContainerSaveData data)
    {
        if (data == null || string.IsNullOrWhiteSpace(data.containerId))
        {
            Debug.LogWarning("저장 실패: data 또는 containerId가 없음");
            return;
        }

        containerStates[data.containerId] = data;
    }

    public bool TryGetContainer(string containerId, out ContainerSaveData data) //5
    {
        if (string.IsNullOrWhiteSpace(containerId))//여기가 살짝 수상한데
        {
            data = null;
            Debug.LogWarning("TryGetContainer 실패: containerId가 없음");
            return false;
        }
        Debug.Log("TryGetContainer 호출: " + containerId);
        return containerStates.TryGetValue(containerId, out data);//여기서 주는건가?? 
    }
}