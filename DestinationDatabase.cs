using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Destination")]

public class DestinationDatabase : ScriptableObject
{
    public string Id;
    public string targetSceneName;
    public string targetSpawnId;
}