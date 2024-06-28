using System;
using UnityEngine;

[Serializable]
public class WaitData : IActionData
{
    [Header("Duration")]
    public float minTime = 1;
    public float maxTime = 1;
}
