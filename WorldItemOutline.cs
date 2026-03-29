using UnityEngine;

public class WorldItemOutline : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private float detectRadius = 5f;
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    private void Update()
    {
        bool playerNear = Physics.CheckSphere(
            transform.position,
            detectRadius,
            playerLayer,
            QueryTriggerInteraction.Ignore
        );

        if (outline != null)
            outline.enabled = playerNear;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
