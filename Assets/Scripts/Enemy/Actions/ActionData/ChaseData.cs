using System;
using UnityEngine;

[Serializable]
public class ChaseData : IActionData
{
    [Header("Movement Settings")]
    public float speedMult = 1f;
    public float minRange = .1f;

    [Header("Drop Item")]
    public DropItemConfig dropConfig;
}
