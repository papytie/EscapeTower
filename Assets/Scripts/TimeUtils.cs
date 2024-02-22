using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtils
{
    public static bool CustomTimer(ref float currentTime, float maxTime, Action method)
    {
        currentTime += Time.deltaTime;
        if (currentTime > maxTime) 
        {
            currentTime = 0;
            method?.Invoke();
            return true;
        }
        return false;
    }

    public static bool CustomTimer(ref float currentTime, float maxTime)
    {
        return CustomTimer(ref currentTime, maxTime, null);
    }

}
