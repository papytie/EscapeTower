using System;
using UnityEngine;

[Serializable]
public class ChargeData : IActionData
{
    public AnimationCurve moveCurve;
    public LayerMask obstructionLayer;
    [Header("Debug")]
    public bool showDebug = false;
    public Color rangeColor = Color.yellow;
    public Color startColor = Color.green;
    public Color targetColor = Color.red;
}
