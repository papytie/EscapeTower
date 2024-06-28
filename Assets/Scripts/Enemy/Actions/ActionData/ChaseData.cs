using System;
using UnityEngine;

[Serializable]
public class ChaseData : IActionData
{
    [Header("Move Settings")]
    public float speedMult = .5f;
    public float minRange = .1f;
}
