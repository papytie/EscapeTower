using System;
using UnityEngine;

[Serializable]
public class FleeData : IActionData
{
    [Header("Movement Settings")]
    public float speedMult = 1f;
    public float maxRange = 2f;
    
    [Header("Drop Item")]
    public DropItemConfig dropConfig;
}
