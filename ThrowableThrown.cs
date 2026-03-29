using UnityEngine;

public class ThrowableThrown : MonoBehaviour
{
    [Header("폭발 설정")]
    public float fuseTime = 3f;
    public float radius = 5f;
    public float explosionForce = 10f;
    //나중
    public float damage = 100f;
    public LayerMask hitLayers;
    [Header("이펙트")]
    public GameObject explosionVFX;

    bool exploded = false;

    public ThrowableItem item;

    public void SetFuseTime(float time)
    {
        fuseTime = Mathf.Max(0.05f, time);
    }
    private void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        SoundManager.Instance.PlayExplosionSound(item);
        switch (item.throwableType)
        {
            case ThrowableType.Grenade:
                {
                    Collider[] cols = Physics.OverlapSphere(transform.position, radius, hitLayers);

                    foreach (Collider col in cols)
                    {
                        Rigidbody rb = col.attachedRigidbody;
                        if (rb != null)
                            rb.AddExplosionForce(explosionForce, transform.position, radius, 0.5f, ForceMode.Impulse);
                    }

                    break;
                }
            case ThrowableType.Smoke:
                {
                    //연막탄 효과 구현
                    break;
                }
        }
        Destroy(gameObject);
    }
}