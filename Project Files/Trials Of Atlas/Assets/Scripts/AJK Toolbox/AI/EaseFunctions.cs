using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseFunctions
{
    public enum EaseType
    {
        CircEaseOut,
        Linear
    }
    
    public static float AccelerateValue(float start, float duration, float currentTime, float startVal, float endVal, EaseType ease)
    {
        if (currentTime > duration)
            return endVal;

        float normTime = currentTime / duration;
        float easedValue;

        switch (ease)
        {
            case EaseType.CircEaseOut:
                easedValue = CircularEaseOut(normTime);
                break;
            case EaseType.Linear:
            default:
                easedValue = Linear(normTime);
                break;
        }

        return Mathf.Lerp(startVal, endVal, easedValue);
    }
    
    private static float CircularEaseOut(float x)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(1 - x, 2));
    }

    private static float Linear(float x)
    {
        return x;
    }
}
