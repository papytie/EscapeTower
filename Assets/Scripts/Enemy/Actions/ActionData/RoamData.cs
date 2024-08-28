using System;
using UnityEngine;

[Serializable]
public class RoamData : IActionData
{
    [Header("Movement Settings")]
    public float speedMult = .5f;
    public float minTime = 1;
    public float maxTime = 5;
    public int maxBounce = 3;
    
    [Header("Drop Item")]
    public DropItemConfig dropConfig;
}
