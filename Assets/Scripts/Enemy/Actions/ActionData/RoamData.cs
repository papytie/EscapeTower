using System;
using UnityEngine;

[Serializable]
public class RoamData : IActionData
{
    [Header("Move Settings")]
    public float speedMult = .5f;
    [Header("Duration")]
    public float minTime = 1;
    public float maxTime = 1;
}
