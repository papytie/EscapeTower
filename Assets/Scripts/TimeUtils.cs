using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeUtils
{
    public static bool CustomTimer(ref float currentTime, float maxTime)
    {
        currentTime += Time.deltaTime;
        if (currentTime > maxTime) 
        {
            currentTime = 0;
            return true;
        }
        return false;
    }
}
