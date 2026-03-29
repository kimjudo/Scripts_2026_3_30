using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("총기 사운드")]
    public AudioSource shootingChannel;
    public AudioSource explosionChannel;
    public AudioSource reloadingChannel;
    public AudioSource emptyMagazineChannel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayShootingSound(GunItem item)
    {
        if (item == null || item.shootingSound == null) return;
        if (!shootingChannel) return;
        shootingChannel.PlayOneShot(item.shootingSound);
    }

    public void PlayReloadSound(GunItem item)
    {
        if (item == null || item.reloadSound == null) return;
        if (!reloadingChannel) return;
        reloadingChannel.PlayOneShot(item.reloadSound);
    }

    public void PlayExplosionSound(ThrowableItem item)
    {
        if (item == null || item.explosionSound == null) return;
        if (!explosionChannel) return;
        explosionChannel.PlayOneShot(item.explosionSound);
    }
}