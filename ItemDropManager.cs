using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public Transform dropPoint; // 플레이어/카메라 앞 빈 오브젝트
    public GameObject worldItemPrefab; // 기본 월드 아이템 프리팹

    private void Awake()
    {
        if (dropPoint == null)
        {
            Debug.LogError("ItemDropManager: dropPoint가 null임", this);
            return;
        }
    }

    public GameObject SpawnWorldItem(Item item, int count)
    {
        if (item == null || count <= 0) return null;

        GameObject last = null;

        for (int i = 0; i < count; i++)
        {
            last = Instantiate(item.worldPrefab, dropPoint.position, Quaternion.identity);
            Debug.Log($"Spawned world item: {item.name}");
        }

        Debug.Log($"Dropped {count} of {item.name} at {dropPoint.position}");
        return last; // 마지막으로 생성된 오브젝트 반환
    }
    public GameObject SpawnWorldItem(Item item, int count, WeaponState state)
    {
        var go = SpawnWorldItem(item, count);

        // 총 상태 저장 (총이 아니면 무시)
        if (go != null && state != null && item is GunItem)
        {
            var s = go.GetComponent<DroppedGunState>();
            if (s == null) s = go.AddComponent<DroppedGunState>();
            s.state = state;
        }

        return go;
    }
}
