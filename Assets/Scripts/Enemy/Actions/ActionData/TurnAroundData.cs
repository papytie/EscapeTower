using System;
using UnityEngine;

[Serializable]
public class TurnAroundData : IActionData
{
    [Header("Movement Settings")]
    public float speedMult = 1f;
    public TurnDirection direction;
    public LookDirection look;
   
    [Header("Drop Item")]
    public DropItemConfig dropConfig;
}
