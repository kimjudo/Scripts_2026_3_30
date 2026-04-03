using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    public Transform[] Points => points;
}