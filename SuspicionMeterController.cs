using UnityEngine;
using UnityEngine.UI;

public class SuspicionMeterController : MonoBehaviour
{
    [SerializeField] private Image ringFill;
    [SerializeField] private Image heart;

    [SerializeField] private Color startColor = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color endColor = Color.white;

    [SerializeField] private float smooth = 10f;

    public Health health;

    private float current;

    private void Start()
    {
        if (heart != null)
            heart.color = startColor;

        if (ringFill != null)
        {
            ringFill.fillAmount = 0f;
            ringFill.color = startColor;
        }
    }

    private void Update()
    {
        if (ringFill == null || heart == null || health == null)
            return;
        
        float target = Mathf.Clamp01(SuspicionRegistry.GetHighestSuspicion());
        current = Mathf.Lerp(current, target, Time.deltaTime * smooth);

        Color c = Color.Lerp(startColor, endColor, current);

        ringFill.fillAmount = current;
        ringFill.color = c;

        if (health.CurrentHealth > 30f)
            heart.color = c;
        else
            heart.color = startColor;
    }
}