
using System;
using System.Diagnostics;

public class Timer
{
    private float timeToComplete;

    private Action onComplete;

    private Action<float> onTick;

    private float currentTime;

    public bool IsComplete { get; private set; }

    public float NormalisedTime
    {
        get { return currentTime / timeToComplete; }
    }

    public Timer(float timeToComplete, Action complete = null, Action<float> tick = null)
    {
        this.timeToComplete = timeToComplete;
        onComplete += complete;
        onTick += tick;
        currentTime = timeToComplete;
        IsComplete = false;
    }

    public void Tick(float deltaTime)
    {
        if (IsComplete)
        {
            return;
        }

        currentTime -= deltaTime;

        onTick?.Invoke(currentTime);

        if (currentTime <= 0)
        {
            onComplete?.Invoke();
            IsComplete = true;
        }
    }

    ~Timer()
    {
        onComplete = null;
        onTick = null;
    }
}