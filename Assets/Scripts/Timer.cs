using System;
using UnityEngine;
public class Timer
{
    public event Action<float> OnTimerUpdate;
    public event Action OnTimerComplete;
    private float duration;
    private float elapsedTime;
    private bool isRunning;

    public Timer(float duration)
    {
        this.duration = duration;
        elapsedTime = 0f;
        isRunning = false;
    }

    public void Start()
    {
        isRunning = true;
        elapsedTime = 0f;
    }

    public void Update()
    {
        if (isRunning == true)
        {
            elapsedTime = elapsedTime + Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            OnTimerUpdate?.Invoke(progress);

            if (elapsedTime >= duration)
            {
                isRunning = false;
                OnTimerComplete?.Invoke();
            }
        }
    }
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
}