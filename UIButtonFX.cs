using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Text")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.75f, 0.75f, 0.75f, 1f);

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;

    void Awake()
    {
        if (!label) label = GetComponentInChildren<TMP_Text>(true);
        if (label) label.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered button: " + gameObject.name);
        if (label) label.color = hoverColor;
        if (audioSource && hoverClip) audioSource.PlayOneShot(hoverClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit button: " + gameObject.name);
        if (label) label.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource && clickClip) audioSource.PlayOneShot(clickClip);
    }
}
