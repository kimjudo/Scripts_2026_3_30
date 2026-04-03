using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("총기 사운드")]
    [SerializeField] private AudioSource shootingChannel;
    [SerializeField] private AudioSource explosionChannel;
    [SerializeField] private AudioSource reloadingChannel;
    [SerializeField] private AudioSource emptyMagazineChannel;

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

    public bool PlayEmptyMagazineSound() // 원래 SoundManager.Instance.emptyMagazineChannel.Play(); 로 weapon에서 불러썻는데 근데 캡슐화를 위해 바꿈
    {
        if (!emptyMagazineChannel) return false;

        emptyMagazineChannel.Play();
        return true;
    }
}