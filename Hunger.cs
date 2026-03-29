using System;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float hungerAmount = 0.3f;
    [SerializeField] private float tickInterval = 10f;

    public float Max => max;
    public float Current { get; private set; }

    public event Action<float, float> onhungerChanged;

    private void Awake()
    {
        Current = max;
        Notify();
    }
    float tickTimer = 0f;
    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer < tickInterval) return;
        if (tickTimer > tickInterval)
        {
            Current = Mathf.Clamp(Current - hungerAmount, 0f, max);
            tickTimer = 0f;
        }
        onhungerChanged?.Invoke(Current, max);
    }

    public void Eat(float amount)
    {
        if (amount <= 0f) return;
        Current = Mathf.Clamp(Current + amount, 0f, max);
        Notify();
    }

    private void Notify()
    {
        onhungerChanged?.Invoke(Current, max);
    }
}
