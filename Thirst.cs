using System;
using UnityEngine;

public class Thirst : MonoBehaviour
{
    [SerializeField] private float max = 100f;
    [SerializeField] private float drainAmount = 0.3f;// 초당 감소량
    [SerializeField] private float tickInterval = 10f; // 몇  초마다 감소할지

    public float Max => max;
    public float Current { get; private set; }

    public event Action<float, float> onThirstChanged; // (cur, max)

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
        Current = Mathf.Clamp(Current - drainAmount, 0f, max);
        tickTimer = 0f;
        onThirstChanged?.Invoke(Current, max);
    }

    public void Drink(float amount)
    {
        if (amount <= 0f) return;
        Current = Mathf.Clamp(Current + amount, 0f, max);
        Notify();
    }

    private void Notify()
    {
        onThirstChanged?.Invoke(Current, max);
    }
}
