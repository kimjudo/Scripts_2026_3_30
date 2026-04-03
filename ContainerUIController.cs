using UnityEngine;


public class ContainerUIController : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Awake()
    {
        if (panel == null)
        {
            Debug.LogWarning("ContainerUIController: panel 참조가 없습니다.");
            return;
        }

        panel.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (panel == null) return;
        panel.SetActive(true);
    }

    public void Close()
    {
        if (panel == null) return;
        panel.SetActive(false);
    }
}
