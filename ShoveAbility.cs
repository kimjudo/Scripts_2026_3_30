using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ShoveAbility : MonoBehaviour
{
    [SerializeField] private float cooldown = 0.5f;
    private float nextUsableTime = 0f;
    [SerializeField] private float radius = 2.0f;
    [SerializeField] private Transform origin;
    [SerializeField] private float maxAngle = 80f;
    [SerializeField] private LayerMask zombieMask;
    [SerializeField] private float shoveForce = 5f;
    [SerializeField] private HandAnimator handAnimator;
    public void TryShove()
    {
        if (Time.time < nextUsableTime) return;

        nextUsableTime = Time.time + cooldown;

        handAnimator.animator.SetTrigger(HandAnimeParams.Shove);
    }


    public void ShoveHitEvent()
    {
        Transform o = origin != null ? origin : transform;

        Collider[] hits = Physics.OverlapSphere(o.position, radius, zombieMask);
        if (hits.Length == 0) return;

        Vector3 dir = o.forward;
        dir.y = 0;
        dir.Normalize();


        foreach (var col in hits)
        {
            var zombie = col.GetComponent<ZombieAI>();

            if (zombie == null) continue;

            zombie.ApplyShove(dir, shoveForce);

            Debug.Log("Shove hit: " + col.name);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Transform o = origin != null ? origin : transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(o.position, radius);

        Vector3 fwd = o.forward;
        fwd.y = 0f;
        fwd.Normalize();

        Vector3 left = Quaternion.Euler(0f, -maxAngle, 0f) * fwd;
        Vector3 right = Quaternion.Euler(0f, maxAngle, 0f) * fwd;

        Gizmos.DrawLine(o.position, o.position + left * radius);
        Gizmos.DrawLine(o.position, o.position + right * radius);
    }
#endif
}
