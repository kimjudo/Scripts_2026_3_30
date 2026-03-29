using System;
using System.Xml.Serialization;
using UnityEngine;

public class Addiction : MonoBehaviour
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float min = 0f;
    [SerializeField] private Health health;

    public float Max => max;
    public float Min => min;
    public float Current { get; private set; }

    public event Action<float, float> onAddictionChanged;

    public void increaseAddiction(float amount)
    {
        Current = Mathf.Clamp(Current + amount, min, max);
        Notify();
    }
    public void DecreaseAddiction(float amount)
    {
        Current = Mathf.Clamp(Current - amount, min, max);
        Notify();
    }
    private void Notify()
    {
        onAddictionChanged?.Invoke(Current, max);
    }

}
