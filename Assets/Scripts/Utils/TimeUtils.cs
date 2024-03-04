using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtils
{
    public static bool CustomTimer(ref float currentTime, float maxTime)
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime) 
        {
            currentTime = 0;
            return true;
        }
        return false;
    }

    public static void StartTimer(ref bool toActivate, ref float endTime, float duration)
    {
        endTime = Time.time + duration;
        toActivate = true;
    }

    public static void CancelTimer(ref bool toCancel, ref float endTime)
    {
        endTime = -1;
        toCancel = false;
    }

}
